using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_MVC.Models;

namespace Project_MVC.Controllers
{
    public class Home_CourseController : Controller
    {
        private readonly ProjectDataBaseContext _context;

        public Home_CourseController (ProjectDataBaseContext context)
        {
            _context = context;
        }

		//上課頁面
		public IActionResult Classstart (int? id) {

			TempData["courseKeyword"] = id;
			var checkcourse = Convert.ToInt32 (TempData["courseKeyword"]);
			var viewclassatartvideo = _context.ViewClassStartVideos.OrderBy (x => x.ChapterId).Where (x => x.CourseId == checkcourse).ToList ();
			ViewBag.ClassStarVideos = viewclassatartvideo;

			//取sessionuserid
			int useridd = Convert.ToInt32 (HttpContext.Session.GetString ("userID"));
			ViewBag.UserIDindex = useridd;

			//評價留言資料
			var classtext = _context.ViewClassTexts.OrderByDescending (x => x.TextTime).Where (x => x.CourseId == checkcourse).ToList ();
			ViewBag.ClassTexts = classtext;
			//這堂課的資料
			var courseID = _context.ViewClassPageTops.Where (x => x.CourseId == checkcourse);

			var Coursebtn = _context.ClassScores.Where (x => x.CourseId == _context.ViewClassPageTops.FirstOrDefault ().CourseId);
			if (true) {

			}
			return View (courseID);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ClassstartQA (ClassText Textdata) {
			// 先確認使用者是否登入
			var userIDs = HttpContext.Session.GetString ("userID");
			if (userIDs == null) {
				return RedirectToAction ("Login", "Home_Login");
			}

			// 驗證表單數據
			if (ModelState.IsValid) {
				// 設定其他需要的屬性（UserID 和 CourseID）

				// 新增留言到資料庫
				_context.Add (Textdata);
				await _context.SaveChangesAsync ();
			}

			// 重新導向到 Classstart Action
			return RedirectToAction ("Classstart", new { id = Textdata.CourseId });
			//return Json ( new { success=true });
		}





		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CourseScore (ClassScore Scores) {

			var userIDs = HttpContext.Session.GetString ("userID");
			if (userIDs == null) {
				return RedirectToAction ("Login", "Home_Login");
			}

			// 驗證表單數據
			if (ModelState.IsValid) {
				// 設定其他需要的屬性（UserID 和 CourseID）

				// 新增留言到資料庫
				_context.Add (Scores);
				await _context.SaveChangesAsync ();
			}

			// 重新導向到 Classstart Action
			return RedirectToAction ("Classstart", new { id = Scores.CourseId });
			//return Json ( new { success=true });

		}









		public IActionResult checkCourse (int? id)
        {

			TempData["courseKeyword"] = id;
            return RedirectToAction("Classpage", "Home_Course");
        }



        public IActionResult Classpage (int? id)
        {

			ViewBag.checkvalue = TempData["checkvalue"];
			//抓session中username
			ViewBag.UserName = HttpContext.Session.GetString("UserName");

            // Home_Index 傳過來的 asp-route-id
            TempData["courseKeyword"] = id;
            var checkcourse = Convert.ToInt32(TempData["coursekeyword"]);

			//導入課程資料表
			var classcourse = _context.ClassCourses.ToList ().FirstOrDefault (x => x.CourseId == checkcourse);
			ViewBag.ClassCourse = classcourse;

			// 從model where 出 courseid = asp-route-id 的課程資訊
			var courseID = _context.ViewClassPageTops.Where(x => x.CourseId == checkcourse);

            // 把string 轉化成 int，並放入 viewbag 中
            int userID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
            ViewBag.UserIDindex = userID;

            var viewChapterCounts = _context.ViewClassChapterCounts.ToList();
            // 找出 CourseId 為 checkCourse 的 ViewClassChapterCount 資料
            var chapterCount = viewChapterCounts.FirstOrDefault(x => x.CourseId == checkcourse);
            // 將 CourseId = checkCourse 的 ChapterCount 傳送到 View
            ViewBag.ChapterCount = chapterCount;

			var classchapter = _context.ClassChapters.OrderByDescending (x => x.ChapterId).Where (x => x.CourseId == checkcourse).ToList ();
			ViewBag.ClassChapter = classchapter;

			//看評價分數，諾沒人評分顯示未有評價
			var classscore = _context.ClassScores.ToList ().FirstOrDefault (x => x.CourseId == checkcourse);
			if (classscore != null) {
				ViewBag.ClassScore = classscore.Score;
			}
			else if (classscore == null) {
				ViewBag.ClassScore = "尚未有評分";
			}
			//看課程評價留言
			var classtext = _context.ViewClassTexts.OrderByDescending (x => x.TextTime).Where (x => x.CourseId == checkcourse).ToList ();
			ViewBag.ClassTexts = classtext;


            int useridd = Convert.ToInt32(HttpContext.Session.GetString("userID"));
            var userimage = _context.Users.ToList().FirstOrDefault(x => x.UserId == useridd);
            ViewBag.UserImage = userimage;

			//看課前問答
			var classqa = _context.ViewClassQabefores.OrderByDescending (x => x.Qatime).Where (x => x.CourseId == checkcourse).ToList ();
			ViewBag.ClassQA = classqa;


			return View(courseID);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ClassQA (ClassQa QA) {

			// 先確認使用者是否登入
			var userIDs = HttpContext.Session.GetString ("userID");
			if (userIDs == null) {
				return RedirectToAction ("Login", "Home_Login");
			}

			// 驗證表單數據
			if (ModelState.IsValid) {
				// 設定其他需要的屬性（UserID 和 CourseID）

				// 新增留言到資料庫
				_context.Add (QA);
				await _context.SaveChangesAsync ();
			}

			// 重新導向到 Classstart Action
			return RedirectToAction ("Classpage", new { id = QA.CourseId });




			////先抓使用者id
			//var userIDs = HttpContext.Session.GetString ("userID");


			////利用 Session id 來判別使用者是否登入 如果沒登入則導回登入頁


			//if (userIDs == null) {
			//	return RedirectToAction ("Login", "Home_Login");
			//}

			////檢查表單是否正確 /若正確則存入資料庫
			//if (ModelState.IsValid) {
			//	_context.Add (QA);
			//	await _context.SaveChangesAsync ();
			//}



			//return Redirect ("Classpage");
		}


		




			[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AddToShoppingcar (ShoppingAddToCart CoursetoShopingCarts) {

			_context.Add (CoursetoShopingCarts);
			_context.SaveChangesAsync ();

			return RedirectToAction ("ClassPage", "Home_Course");
		}



		[HttpPost]
		public async Task<IActionResult> CourseContents (ClassCourse updatedCourseContent) {
			var courseIDindex = HttpContext.Session.GetString ("courseID");
			int courseId = Convert.ToInt32 (courseIDindex);

			// 查询数据库中已有的数据
			var existingCourse = await _context.ClassCourses.FirstOrDefaultAsync (c => c.CourseId == courseId);

			if (existingCourse != null) {
				// 更新需要更新的属性
				existingCourse.CourseContent = updatedCourseContent.CourseContent;
				// 如果有其他需要更新的属性，也在这里进行设置

				await _context.SaveChangesAsync ();
			}

			return RedirectToAction ("Index", "Home_Login");
		}


		public IActionResult Test1 (string id)
        {

            TempData["test"] = id;
            return RedirectToAction("ClassPage", "Home_Course");
        }




		public IActionResult Classupload1 () {
			var userID = HttpContext.Session.GetString ("userID");
			var UserName = HttpContext.Session.GetString ("UserName");
			ViewBag.userName = UserName;
			ViewBag.UserID = userID;

			//如果使用者未登入會被引導至登入頁面
			if (string.IsNullOrEmpty (userID)) {

				return RedirectToAction ("Login", "Home_Login");

			}
			else {
				return View ();
			}
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Classupload1 (ClassCourse CourseData, IFormFile img, IFormFile video) {

			//判斷是否有上傳圖片
			if (img != null && img.Length > 0) {
                //處理上傳圖片資料

                Random random = new Random ();
                int randomTwoDigitNumber = random.Next (10, 100); // 產生一個介於10（包含）到100（不包含）之間的隨機數字
                string fileName = randomTwoDigitNumber + "Courseimg.jpg";

				string YourDesiredPath = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot", "Images");

				// 確保目錄存在，如果不存在則創建它
				if (!Directory.Exists (YourDesiredPath)) {
					Directory.CreateDirectory (YourDesiredPath);
				}

				// 指定存放位置
				string filePath = Path.Combine (YourDesiredPath, fileName);

				// 將檔案儲存到指定位置
				using (var stream = new FileStream (filePath, FileMode.Create)) {
					img.CopyTo (stream);
				}
				string WebImgUrl = "/Images/" + fileName;

				//ViewBag.ImgUrl = WebImgUrl;
				CourseData.CourseImageUrl = WebImgUrl;
			}

			//判斷是否有上傳影片
			if (video != null && video.Length > 0) {
				string videoName = CourseData.CourseName + "Coursevideo.mp4";
				string YourVideoPath = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot", "CourseVideo");

				// 確保目錄存在，如果不存在則創建它
				if (!Directory.Exists (YourVideoPath)) {
					Directory.CreateDirectory (YourVideoPath);
				}
				// 指定存放位置
				string videoPath = Path.Combine (YourVideoPath, videoName);

				// 將檔案儲存到指定位置
				using (var stream = new FileStream (videoPath, FileMode.Create)) {
					video.CopyTo (stream);
				}
				string WebVideoUrl = "/CourseVideo/" + videoName;
				CourseData.CourseIntroVideo = WebVideoUrl;
			}

			var userIDindex = HttpContext.Session.GetString ("userID");
			CourseData.UserId = Convert.ToInt32 (userIDindex);

			//if (ModelState.IsValid) {
			//將資料加入資料庫中
			_context.Add (CourseData);
			await _context.SaveChangesAsync ();
			HttpContext.Session.SetString ("courseID", CourseData.CourseId.ToString ());
			HttpContext.Session.SetString ("courseIntro", CourseData.CourseId.ToString ());

			//return RedirectToAction ("Classupload2");
			//}

			return View ();
		}






		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ClassChaptertoPost (List<ClassChapter> ClassChapters, List<IFormFile> CourseVideo) {
			var courseID = HttpContext.Session.GetString ("courseID");

			for (int i = 0; i < ClassChapters.Count && i < CourseVideo.Count; i++) {
				var chapter = ClassChapters[i];
				var video = CourseVideo[i];

				chapter.CourseId = Convert.ToInt32 (courseID);

				// 判斷是否有上傳影片
				if (video != null && video.Length > 0) {
					string videoName = chapter.ChapterTitle + "Coursevideo.mp4";
					string YourVideoPath = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot", "CourseVideo");

					// 確保目錄存在，如果不存在則創建它
					if (!Directory.Exists (YourVideoPath)) {
						Directory.CreateDirectory (YourVideoPath);
					}

					// 指定存放位置
					string videoPath = Path.Combine (YourVideoPath, videoName);

					// 將檔案儲存到指定位置
					using (var stream = System.IO.File.Create (videoPath)) {
						await video.CopyToAsync (stream);
					}
					//將章節名稱指定成資料列資料
					string WebVideoUrl = "/CourseVideo/" + videoName;
					chapter.CourseVideo = WebVideoUrl;
				}

				_context.Add (chapter);
			}
			await _context.SaveChangesAsync ();
			return RedirectToAction ("Classupload1","Home_Course");
		}






		public IActionResult Classupload2 () {
			var CourseIDindex = HttpContext.Session.GetString ("courseID");
			var CourseIntroindex = HttpContext.Session.GetString ("courseIntro");
			var UserName = HttpContext.Session.GetString ("UserName");
			//var CourseImg = HttpContext.Session.GetString ("courseImg");
			if (CourseIDindex != null) {
				ViewBag.courseIDindex = CourseIDindex;
				ViewBag.courseIntroindex = CourseIntroindex;
				ViewBag.userName = UserName;

			}
			if (CourseIDindex == null) {
				ViewBag.userIDindex = null;
				ViewBag.courseIntroindex = null;
				ViewBag.userName = null;
			}

			int courseID = Convert.ToInt32 (CourseIDindex);
			var courseData = _context.ClassCourses.Find (courseID);
			string courseName = courseData.CourseName;
			string courseIntro = courseData.CourseIntro;
			ViewBag.CourseName = courseName;
			ViewBag.CourseIntro = courseIntro;


			return View ();
		}





		public IActionResult Classupload3 ()
        {
            return View();
        }
        public IActionResult Course_Edit ()
        {
            return View();
        }
		public IActionResult test() {
			return View ();
		}

		//#################  政修新增部分編輯及新增老師資料  ########/////////////////////////////////////////////////////////////


		//顯示老師資料明細畫面
		public async Task<IActionResult> TeacherData()
        {


            var userid = HttpContext.Session.GetString("userID");


            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userid, out userIDnumber);
			ViewBag.userIDnumber = userIDnumber;	

			var projectDataBaseContext = _context.UserTeachers.Where(x => x.UserId == userIDnumber);
            return View(await projectDataBaseContext.ToListAsync());
        }





        //老師新增資料填寫表格畫面
        public IActionResult TeacherDataCreate(int? id)
        {

            var userid = HttpContext.Session.GetString("userID");

            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userid, out userIDnumber);

            ViewBag.userid = userIDnumber;

			var checkdata = _context.UserTeachers.Where(x => x.UserId == userIDnumber).FirstOrDefault();


			//判別是否已有新增老師資料，如果有責跳至明細畫面(可修改資料)，否則跳至新增畫面去新增老師資料
			if (checkdata == null)
			{
                return View();


            }
			else {

				var ids = checkdata.UserId;

                return RedirectToAction("TeacherData", "Home_Course");

            }

            return View();
        }



		//處理老師填寫表格資料程式
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeacherDataCreate([Bind("TeacherId,UserId,UserIntro1,UserIntro2,UserBgimage")] UserTeacher userTeacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userTeacher);
                await _context.SaveChangesAsync();
                return RedirectToAction("TeacherData", "Home_Course");
            }
            return RedirectToAction("TeacherData", "Home_Course");
        }



        //顯示修改畫面填寫表單
        public async Task<IActionResult> TeacherDataEdit(int? id)
        {

            var userid = HttpContext.Session.GetString("userID");

            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userid, out userIDnumber);

			ViewBag.userIDnumber = userIDnumber;	


            if (id == null || _context.UserTeachers == null)
            {
                return NotFound();
            }

            var userTeacher = await _context.UserTeachers.FindAsync(id);
            if (userTeacher == null)
            {
                return NotFound();
            }
           
            return View(userTeacher);
        }


		//處理修改表單資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeacherDataEdit(int id, [Bind("TeacherId,UserId,UserIntro1,UserIntro2,UserBgimage")] UserTeacher userTeacher)
        {
           
    
                    _context.Update(userTeacher);
                    await _context.SaveChangesAsync();


                return RedirectToAction("TeacherData", "Home_Course");
            
           
        }



        //#################  政修新增部分編輯及新增老師資料  ########/////////////////////////////////////////////////////////////






    }
}
