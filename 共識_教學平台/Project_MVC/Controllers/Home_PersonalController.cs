using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_MVC.Models;
using System.Linq;

namespace Project_MVC.Controllers
{
    public class Home_PersonalController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProjectDataBaseContext _projectDataBaseContext;
        public Home_PersonalController (ILogger<HomeController> logger, ProjectDataBaseContext ProjectDataBaseContext)
		{
			_logger = logger;
            _projectDataBaseContext = ProjectDataBaseContext;
        }

        public ActionResult checkTeacher(int? id) 
        {
            TempData["checkTeacherID"] = id;
            return RedirectToAction("Personal_Teacher", "Home_Personal");
        }





		public IActionResult Personal_Teacher (int? id)
        {


            // Shopping_Teacher 傳過來的 asp-route-id
            TempData["checkTeacherID"] = id;

            //政修 修改部分---老師大頭照要去抓使用者資料表的老師大頭照///////////////////////////

            var image = _projectDataBaseContext.Users.Where(x=>x.UserId==id).Select(x=>x.UserImageUrl).FirstOrDefault();
            ViewBag.Image = image;  

            ///////////////////////////////////////////////////////////////////

            var checktecher = Convert.ToInt32(TempData["checkTeacherID"]);

            // 從model where 出 courseid = asp-route-id 的課程資訊
            var teacherID = _projectDataBaseContext.ViewTeacherCourses.Where(x =>x.UserId == checktecher).ToList();

            return View(teacherID);
        }
        public IActionResult Personal_TeacherEdit ()
        {
            return View();
        }

        //public IActionResult Personal_Student ()
        //{
        //    return View();
        //}

        //會員登入成功後的個人頁面，機器人幫我建立的，我再加上自己需要的東西
        // GET: Users
        public async Task<IActionResult> Personal_Student ()
        {
            //如果使用者沒有登入但是想看個人頁面(隸屬於會員專區)，會被引導到登入頁面
            var id = HttpContext.Session.GetString("userID");
            bool isParsed = int.TryParse(id, out int number);

            ViewBag.userid = number;

            //開始判斷
            if (string.IsNullOrEmpty(id))
            {
                //重新導向的功能，我要讓使用者登入之後記得回家，也就是回到個人頁面。
                TempData["ReturnUrl"] = "/Home_Login/Index";
                string userName = HttpContext.Session.GetString("userName") ?? "Guest";
                if (userName == "Guest")
                {
                    return Redirect("/Home_Login/Login");
                }
                return RedirectToAction("Login", "Home_Login");
            }

            //用viewbag裝
            ViewBag.Name = HttpContext.Session.GetString("UserName");

            //擷取成功登入後存入Session的使用者名稱
            var PersonalName = HttpContext.Session.GetString("UserName");

            //顯示個人的大頭照
            List<User> PersonalImage = _projectDataBaseContext.Users.Where(x => x.UserName == PersonalName).ToList();
            ViewBag.ShowPersonalImage = PersonalImage;

            //只能看到自己的資料
            var onlySeePersonalInfo = HttpContext.Session.GetString("UserEmail");


            //要能看到自己買的課程
            //var PersonalBuiedCourses = HttpContext.Session.GetString("BuiedCourses");
            //List<ViewPersonalIndexBuiedCourse> PersonalBuiedCourses = _context.Users.Where

            return _projectDataBaseContext.Users != null ?
                        View(await _projectDataBaseContext.Users.Where(x => x.UserEmail == onlySeePersonalInfo).ToListAsync()) :
                        Problem("Entity set 'ProjectDataBaseContext.Users'  is null.");
        }

        public IActionResult Personal_StudentEdit ()
        {
            return View();
        }

    }
}

