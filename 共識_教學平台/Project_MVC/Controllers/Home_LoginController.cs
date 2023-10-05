using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_MVC.Models;
using static Azure.Core.HttpHeader;

namespace MyPrpject_Post.Controllers
{
    public class Home_LoginController : Controller
    {

        //建立接收前端登入post的資料模型 參數名稱要跟前端表單名稱相同才能對接
        public class MemberModel
        {
            public string UserEmail { get; set; }
            public string Password { get; set; }
        }

        private readonly ProjectDataBaseContext _context;

        public Home_LoginController (ProjectDataBaseContext context)
        {
            _context = context;
        }


        //======================================================================
        //會員登入
        public IActionResult Login ()
        {
            //擷取錯誤訊息，告訴使用者已經註冊過，請直接登入。
            ViewBag.PleaseLoginDirectly = HttpContext.Session.GetString("LoginDirectly");

            //提醒使用者已經註冊成功，可以登入了。
            ViewBag.RegisterSucceed = HttpContext.Session.GetString("RegisterSucceed");
            return View();
        }


        //會員登入：登入處理機制
        [HttpPost]
        //把前端接收的表單存到這裡(formData)
        public IActionResult Login (MemberModel formData)
        {
            var formDatas = formData;

            //檢查使用者登入表單的資料→去跟資料庫做比對(這裡是比對信箱跟密碼)
            var checkEmail = _context.Users.Where(
                x => x.UserEmail == formDatas.UserEmail).FirstOrDefault();
            var checkPWD = checkEmail.UserPassword == formDatas.Password;

            //開始進行身分驗證
            if (checkEmail != null && checkPWD == true)
            {

                //通過驗證然後建立可以"跨網頁"存取的session，這邊建立然後存取userID、UserName跟UserPassword
                HttpContext.Session.SetString("userID", checkEmail.UserId.ToString());
                HttpContext.Session.SetString("UserEmail", checkEmail.UserEmail.ToString());
                HttpContext.Session.SetString("UserName", checkEmail.UserName.ToString());
                HttpContext.Session.SetString("password", checkEmail.UserPassword.ToString());
                HttpContext.Session.SetString("ImagesUrl", checkEmail.UserImageUrl.ToString());

                //這個是想要看購物車但沒登入的話會來的地方(因為購物車只有會員才可以看)，阿如果登入了，就會回家，也就是購物車的地方
                if (TempData["ReturnUrl"] != null)
                {
                    string returnUrl = TempData["ReturnUrl"].ToString();
                    TempData.Remove("ReturnUrl");
                    return Redirect(returnUrl);
                }

                //若驗證沒問題，則進入指定頁面
                return RedirectToAction("Index", "Home_Login");
            }
            else
            {
                HttpContext.Session.SetString("WrongAns", "帳號或密碼有誤，請重新輸入");
                ViewBag.WrongWarning = HttpContext.Session.GetString("WrongAns");
                return View();
            }
        }


        //========================================================================
        // GET: Userdatums/Create
        //會員註冊
        public IActionResult Create ()
        {
            return View();
        }


        //會員註冊：接收使用者填寫的註冊表單資料 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (User UserData, IFormFile img)
        {

            //要檢查資料是不是已經被註冊過，這裡用信箱跟密碼檢查
            var checkEmail = _context.Users.Where(x => x.UserEmail == UserData.UserEmail).FirstOrDefault();
            var checkPWD = _context.Users.Where(x => x.UserPassword == UserData.UserPassword).FirstOrDefault();


            //開始檢查判斷資料是不是已經註冊過
            if (checkEmail == null && checkPWD == null)
            {


                //如果使用者有上傳照片
                if (img != null && img.Length > 0)
                {

                    var inputID = UserData.UserName;
                    string fileName = inputID + "userimg.jpg";

                    string YourDesiredPath = "wwwroot/images";

                    // 指定存放位置
                    string filePath = Path.Combine(YourDesiredPath, fileName);

                    // 將檔案儲存到指定位置
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        img.CopyTo(stream);
                    }

                    string WebImgUrl = "/images/" + fileName;
                    UserData.UserImageUrl = WebImgUrl;
                    //如果沒有上傳照片，就用我給他的，也就是說可以不用自己先預設大頭照
                }
                else
                {
                    UserData.UserImageUrl = "/images/Userimg.jpg";
                }

                //如果沒有註冊過，就把資料存到資料庫中
                _context.Add(UserData);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

                HttpContext.Session.SetString("RegisterSucceed", "註冊成功，請登入會員");
                //然後跳轉到登入頁面，請使用者直接登入
                return Redirect("Login");
            }
            else
            {

                //如果已經註冊過，就用Session存取錯誤訊息
                HttpContext.Session.SetString("LoginDirectly", "已經註冊過，請直接登入會員");
                // ViewBag.Logined = HttpContext.Session.GetString("logined");
                return Redirect("Login");
            }
            return View(UserData);
        }


        //============================================================
        //會員登入成功後的個人頁面，用機器人幫我建立的，再加上我需要的東西
        // GET: Userdatums
        public async Task<IActionResult> Index ()
        {

            //如果使用者沒有登入但是想看個人頁面(隸屬於會員專區)，會被引導到登入頁面
            var id = HttpContext.Session.GetString("userID");
            bool isParsed = int.TryParse(id, out int number);




            //要在"我的課程"區可以看到自己買的課程照片+課程名稱+上課超連結
            var ShowBuiedCourses = _context.ViewOrderDetailsInIndices.Where(p => p.UserId == number).ToList();
            //等等放進迴圈跑
            ViewBag.BuiedCoursesInIndex = ShowBuiedCourses;

            ViewBag.userName = HttpContext.Session.GetString("UserName");
            var myteacherclass = _context.ClassCourses.OrderByDescending (x => x.CourseId).Where (p => p.UserId == Convert.ToInt32 (id)).ToList ();
            ViewBag.MyTeacherClass = myteacherclass;

            ViewBag.userid = number;

            //開始判斷如果使用者沒有登入但是想看個人頁面(隸屬於會員專區)，會被引導到登入頁面
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
            List<User> PersonalImage = _context.Users.Where(x => x.UserName == PersonalName).ToList();
            ViewBag.ShowPersonalImage = PersonalImage;

            //只能看到自己的資料
            var onlySeePersonalInfo = HttpContext.Session.GetString("UserEmail");

            return _context.Users != null ?
                          View(await _context.Users.Where(x => x.UserName == PersonalName).ToListAsync()) :
                          Problem("Entity set 'ProjectDataBaseContext.Users'  is null.");
        }


        //=========================================================================
        //編輯個人頁面：機器人幫我創建的，我再加以修改
        // GET: Userdatums/Edit/5
        public async Task<IActionResult> Edit (int? id)
        {

            //擷取成功登入後存入Session的使用者名稱跟使用者ID
            var PersonalName = HttpContext.Session.GetString("UserName");
            var userID = HttpContext.Session.GetString("userID");

            ViewBag.UserID = userID;

            //顯示個人的大頭照
            var PersonalImage = _context.Users.Where(x => x.UserName == PersonalName).Select(i => i.UserImageUrl).FirstOrDefault();
            ViewBag.ShowPersonalImage = PersonalImage;

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Userdatums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (User userdatum, IFormFile img)
        {
            //if (id != userdatum.UserId) { return NotFound();}


            //如果使用者有上傳照片
            if (img != null && img.Length > 0)
            {

                var inputID = userdatum.UserName;
                string fileName = inputID + "userimg.jpg";
                string YourDesiredPath = "wwwroot/images";
                // 指定存放位置
                string filePath = Path.Combine(YourDesiredPath, fileName);

                // 將檔案儲存到指定位置
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    img.CopyTo(stream);
                }

                // 將檔案儲存到指定位置
                //using (var stream = new FileStream(filePath, FileMode.)) {
                //    user.UserImageUrl.CopyTo("img/123");
                //}
                string WebImgUrl = "/images/" + fileName;
                //await System.IO.File.WriteAllLines(filePath, WebImgUrl);

                userdatum.UserImageUrl = WebImgUrl;

                //如果沒有上傳照片，就用我給他的
            }
            else
            {
                userdatum.UserImageUrl = "/images/Userimg";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userdatum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserdatumExists(userdatum.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userdatum);
        }

        public IActionResult LogOut ()
        {
            HttpContext.Session.Remove("userID");
            return Redirect("/Home/Home_Index");
        }








        //====================以下這些暫時不會用到====================
        //====================以下這些暫時不會用到====================
        //====================以下這些暫時不會用到====================
        // GET: Userdatums/Details/5
        public async Task<IActionResult> Details (int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var userdatum = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userdatum == null)
            {
                return NotFound();
            }

            return View(userdatum);
        }


        // GET: Userdatums/Delete/5
        public async Task<IActionResult> Delete (int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var userdatum = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userdatum == null)
            {
                return NotFound();
            }

            return View(userdatum);
        }

        // POST: Userdatums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed (int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'TestprojectContext.Userdata'  is null.");
            }
            var userdatum = await _context.Users.FindAsync(id);
            if (userdatum != null)
            {
                _context.Users.Remove(userdatum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserdatumExists (int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

    }
}
