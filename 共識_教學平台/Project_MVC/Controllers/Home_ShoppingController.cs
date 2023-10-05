using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_MVC.Models;

namespace Project_MVC.Controllers
{
    public class Home_ShoppingController : Controller
    {
        private readonly ProjectDataBaseContext _projectDataBaseContext;
        public Home_ShoppingController (ProjectDataBaseContext ProjectDataBaseContext)
        {
            _projectDataBaseContext = ProjectDataBaseContext;
        }

        public IActionResult Shopping_Index ()
        {

            var courses = _projectDataBaseContext.ViewUserCourses.Where
                (
                    p => p.CourseName != null &&
                         p.UserCourseImageUrl != null &&
                         p.CourseIntro != null &&
                         p.CourseDuration !=null &&
                         p.CoursePrice !=null
                ).ToList();
            ViewBag.Cart = courses;
            
            return View(courses);
        }

        //課程搜尋框
        [HttpPost]
        public async Task<IActionResult> Shopping_Index (string CourseKeyword)
        {
            return _projectDataBaseContext.ViewUserCourses != null ?
                View(await _projectDataBaseContext.ViewUserCourses.Where
                (
                     x => x.CourseCategoryName.Contains(CourseKeyword)
                ).ToListAsync()) :
                Problem("Entity set 'ProjectDataBaseContext.ViewUserCourses'  is null.");
        }

        [HttpPost]
        public IActionResult CheckCourseExists (int courseId)
        {
            var id = HttpContext.Session.GetString("userID");

            bool isParsed = int.TryParse(id, out int number);
            bool courseExists = _projectDataBaseContext.ShoppingAddToCarts.Any(c => c.CourseId == courseId && c.UserId == number);

            if (!courseExists)
            {
                // 如果CourseId不存在，則新增一筆記錄
                var newCourse = new ShoppingAddToCart
                {
                    CourseId = courseId,
                    UserId = number,
                };

                // 將新的Course記錄加入資料庫並保存更改
                _projectDataBaseContext.ShoppingAddToCarts.Add(newCourse);
                _projectDataBaseContext.SaveChanges();

                // 返回JSON結果，表示CourseId不存在並已經新增
                return Json(new { exists = false });
            }

            // 返回JSON結果，表示CourseId已經存在
            return Json(new { exists = true });
        }

        //這是"加入購物車"的那個"行為"，所以沒有檢視表，加入後，"購物區加入購物車"資料表，就會多一筆訂單
        //public IActionResult AddToCart ()
        //{
        //    //只能看到自己的
        //    var id = HttpContext.Session.GetString("userID");
        //    bool isParsed = int.TryParse(id, out int number);

        //    //開始判斷如果使用者沒有登入但是想加入購物車(隸屬於會員專區)，會被引導到登入頁面
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        //重新導向的功能，我要讓使用者登入之後記得回家，也就是回到課程瀏覽頁面(ShoppingCourseBrowse)。
        //        TempData["ReturnUrl"] = "/Home_Shopping/Shopping_Index";
        //        string userName = HttpContext.Session.GetString("userName") ?? "Guest";
        //        if (userName == "Guest")
        //        {
        //            return Redirect("/Home_Login/Login");
        //        }
        //        return RedirectToAction("Shopping_Index", "Home_Shopping");
        //    }

        //    int s = Convert.ToInt32(HttpContext.Request.Query["CID"]);
        //    //var egg = _context.ClassCourses.Where(x => x.CourseId == s).Select(j => j);

        //    bool isOrderExixts = _projectDataBaseContext.ShoppingAddToCarts.Any(cart => cart.UserId == number && cart.CourseId == s);

        //    //拿來防堵使用者重複加入購物車的機制
        //    if (isOrderExixts)
        //    {
        //        //如果這門課已經加入購物車，用sweetalert提醒使用者不要再加了
        //        TempData["DuplicateOrder"] = true;
        //    }
        //    else
        //    {
        //        //如果還沒購買，就丟到購物車沒問題
        //        var applecat = new ShoppingAddToCart();
        //        //applecat.UserId = 2; //先寫死
        //        applecat.UserId = number;
        //        applecat.CourseId = s;
        //        _projectDataBaseContext.Add(applecat);
        //        _projectDataBaseContext.SaveChanges();
        //        TempData["AddedToCart"] = true;
        //    }
        //    return Redirect("Shopping_Index");
        //}

        public IActionResult Shopping_Children ()
        {
            var courses = _projectDataBaseContext.ViewUserCourses.ToList();
            return View(courses);
        }
        //課程搜尋框
        [HttpPost]
        public async Task<IActionResult> Shopping_Children (string CourseKeyword)
        {
            return _projectDataBaseContext.ViewUserCourses != null ?
                View(await _projectDataBaseContext.ViewUserCourses.Where
                (
                    x => x.CourseName.Contains(CourseKeyword)
                ).ToListAsync()) :
                Problem("Entity set 'ProjectDataBaseContext.ViewUserCourses'  is null.");
        }

        public IActionResult Shopping_TeacherList ()
        {
            var teacherlist = _projectDataBaseContext.ViewUserTeachers.Where
                (
                    p => p.UserIntro1 != null &&
                         p.UserIntro2 != null &&
                         p.TeacherImage != null
                ).ToList();
            return View(teacherlist);
        }
        [HttpPost]
        public async Task<IActionResult> Shopping_TeacherList (string TeacherKeyword)
        {
            return _projectDataBaseContext.ViewUserTeachers != null ?
                View(await _projectDataBaseContext.ViewUserTeachers.Where(x => x.UserTeacherName.Contains(TeacherKeyword)).ToListAsync()) :
                Problem("Entity set 'ProjectDataBaseContext.ViewUserTeachers'  is null.");
        }
        public IActionResult Shopping_TeacherPersonal ()
        {
            return RedirectToAction("Home_Personal", "Personal_Teacher");
        }
    }
}
