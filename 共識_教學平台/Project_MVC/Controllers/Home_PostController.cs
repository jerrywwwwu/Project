using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_MVC.Models;
using System.Security.Cryptography;


namespace Project_MVC.Controllers


{
    public class Home_PostController : Controller
    {
        private readonly ProjectDataBaseContext _context;


        public Home_PostController(ProjectDataBaseContext dataBaseContext)
        {
            _context = dataBaseContext;
        }



        //###########################################   技能討論區文章專區 -----------------------------------------


        //討論區首頁顯示部分------------------->>>根據不同需求變換顯示資料畫面
        public async Task<IActionResult> Post_Index()

        {


            //抓取使用者userID資訊
            var userIDindex = HttpContext.Session.GetString("userID");

            if (userIDindex != null)
            {
                ViewBag.userIDindex = userIDindex;
            }


            if (userIDindex == null)
            {
                ViewBag.userIDindex = null;
            }




            //抓留言資料給前端炫染用-->按照時間排序
            var CommentData = _context.ViewPostComments.OrderByDescending(x => x.CommentTime).ToList();
            ViewBag.CommentData = CommentData;





            //要用來判別數值是否存在所以要先將資料格式轉成文字
            string Category = TempData["Category"] as string;


            //根據傳來的名子顯示該作者的文章內容

            if (Category != null)
            {

                ViewBag.title = "屬於 " + Category + " 的相關文章";  //ViewBag.title -->網頁大標名稱


                var projectDataBaseContext = _context.ViewPosts;

                //根據關鍵字顯示
                var result = await projectDataBaseContext.Where(x => x.PostCategory == Category).OrderByDescending(x => x.PostId).ToListAsync();

                return View(result);



            }



            //要用來判別數值是否存在所以要先將資料格式轉成文字
            string PosterName = TempData["PosterName"] as string;


            //根據傳來的名子顯示該作者的文章內容

            if (PosterName != null)
            {

                ViewBag.title = "屬於 " + PosterName + " 的所有文章";  //ViewBag.title -->網頁大標名稱


                var projectDataBaseContext = _context.ViewPosts;

                //根據關鍵字顯示
                var result = await projectDataBaseContext.Where(x => x.UserName == PosterName).OrderByDescending(x => x.PostId).ToListAsync();

                return View(result);



            }




            //要用來判別數值是否存在所以要先將資料格式轉成文字
            string GetArticle = TempData["GetArticle"] as string;


            //根據傳來的名子顯示該作者的文章內容

            if (GetArticle != null)
            {

                ViewBag.title = GetArticle;  //ViewBag.title -->網頁大標名稱


                var projectDataBaseContext = _context.ViewPosts;

                //根據關鍵字顯示
                var result = await projectDataBaseContext.Where(x => x.PostTitle == GetArticle).OrderByDescending(x => x.PostId).ToListAsync();

                return View(result);

            }





            //根據關鍵字顯示文章內容，keyWord 從其他action傳來 ,根據關鍵字去篩選顯示資料表資料

            //要用來判別數值是否存在所以要先將資料格式轉成文字
            string keyword = TempData["keyWord"] as string;


            //判別有無關鍵字，無關鍵字則顯示所有資料 ，有關鍵字則顯示資料有關鍵字的相關文章

            if (keyword == null)
            {
                ViewBag.title = "技能討論貼文專區";  //ViewBag.title -->網頁大標名稱
                var projectDataBaseContext = _context.ViewPosts;
                var result = await projectDataBaseContext.OrderByDescending(x => x.PostId).ToListAsync();
                return View(result);   //顯示所有文章資料
            }
            else if (keyword == "!熱門文章!")
            {


                ViewBag.title = "最熱門文章";
                var projectDataBaseContext = _context.ViewPosts;

                //關鍵字為熱門文章 則顯示資料庫中文章評比分數最高的前十篇文章
                var result = await projectDataBaseContext.OrderByDescending(x => x.ValuePoint).Take(10).ToListAsync();
                return View(result);

            }
            else
            {

                if (keyword == "平面設計")
                {

                    ViewBag.title = "平面設計類文章";

                }
                else if (keyword == "程式")
                {
                    ViewBag.title = "程式學習類文章";

                }
                else if (keyword == "英文")
                {
                    ViewBag.title = "英語學習類文章";
                }
                else
                {

                    ViewBag.title = "標籤搜尋:" + keyword;

                }


                var projectDataBaseContext = _context.ViewPosts;

                //根據關鍵字顯示
                var result = await projectDataBaseContext.Where(x => x.PostContent.Contains(keyword)).OrderByDescending(x => x.PostId).ToListAsync();

                return View(result);



            }


        }

        //##--------------------------#***--  接口專區  #--------------------------#***--↓↓↓↓↓↓

        //處理搜尋資料的controller --->處理完用TempData將 關鍵字 傳至Post_Index---------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(string searchData)
        {

            TempData["keyWord"] = searchData;
            return RedirectToAction("Post_Index", "Home_Post");
        }

        //-----------------------------------------------------------------------




        //處理側邊攔分類搜尋資料的controller --->處理完用TempData將 關鍵字 傳至Post_Index---------

        public IActionResult checkPost(string? id)
        {

            TempData["keyWord"] = id;
            return RedirectToAction("Post_Index", "Home_Post");
        }

        //-----------------------------------------------------------------------





        //處理標籤tag搜尋資料的controller --->處理完用TempData將 關鍵字 傳至Post_Index---------

        public IActionResult checkTag(string? id)
        {

            TempData["keyWord"] = id;
            return RedirectToAction("Post_Index", "Home_Post");
        }

        //-----------------------------------------------------------------------




        //處裡 按下使用者姓名搜尋其資料的controller --->處理完用TempData將 關鍵字 傳至Post_Index---------

        public IActionResult checkPosterName(string? id)
        {

            TempData["PosterName"] = id;
            return RedirectToAction("Post_Index", "Home_Post");
        }

        //-----------------------------------------------------------------------

        //抓文章分類去顯示文章
        public IActionResult checkCategory(string? id)
        {

            TempData["Category"] = id;
            return RedirectToAction("Post_Index", "Home_Post");
        }

        //-----------------------------------------------------------------------




        //抓文章標題去顯示文章
        public IActionResult GetArticle(string? id)
        {

            TempData["GetArticle"] = id;
            return RedirectToAction("Post_Index", "Home_Post");
        }

        //-----------------------------------------------------------------------









        //###########################################  技能討論區文章專區 -----------------------------------









        //########################################### 發文專區 ------------------------------

        public IActionResult Post_InputData()
        {
            var userID = HttpContext.Session.GetString("userID");

            ViewBag.UserID = userID;



            //如果使用者未登入會被引導至登入頁面
            if (string.IsNullOrEmpty(userID))
            {

                return RedirectToAction("Login", "Home_Login");

            }
            else
            {


                return View();


            }
        }




        //使用者發文後台 --->寫完直跳轉至文章專區 ，其中UserId是抓使用者登入的 Session 資料 (處理發文章後台部分)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post_InputData([Bind("PostId,UserId,PostCategory,PostTitle,PostPublishTime,PostContent,PostTag1,PostTag2,PostTag3")] Post post, IFormFile PostrImgUrls)
        {
            var userIDs = HttpContext.Session.GetString("userID");

            //將session抓的UserId強制換為數字變數(因為資料庫UserId只能存數字資料)
            int userIDnumber;
            bool success = int.TryParse(userIDs, out userIDnumber);

            //將 post 模型中的 UserId自動附值(因為使用者表單沒有填)
            post.UserId = userIDnumber;




            //判斷是否使用者有傳照片
            if (PostrImgUrls != null)
            {

                //處理上傳圖片資料
                Random random = new Random();
                Random random2 = new Random();

                // Generate a random integer between a specified range (e.g., 1 and 100).
                int randomNumber = random.Next(1, 101);
                int randomNumber2 = random2.Next(1, 101);


                var inputID = randomNumber;
                var inputID2 = randomNumber2;


                string fileName = inputID + inputID2 + "posttitleimg.jpg";
                string YourDesiredPath = "wwwroot/images";


                // 指定存放位置
                string filePath = Path.Combine(YourDesiredPath, fileName);

                // 將檔案儲存到指定位置
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    PostrImgUrls.CopyTo(stream);
                }

                string WebImgUrl = "/images/" + fileName;
                post.PostrImgUrl = WebImgUrl;


            }
            else
            {

                post.PostrImgUrl = null;

            }



            _context.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Post_Index", "Home_Post");  // ----導向貼文首頁



            return View(post);
        }

        //########################################### 發文專區 ------------------------------







        //########################################### 處理留言表單 ------------------------------

        ///處理留言資料
        [HttpPost]
        public IActionResult comments( string CommentContent , int UserId ,int PostId)
        {

            
            //先抓使用者id
            var userIDs = HttpContext.Session.GetString("userID");

            //利用 Session id 來判別使用者是否登入 如果沒登入則導回登入頁


            if (userIDs == null)
            {
                return RedirectToAction("Login", "Home_Login");
            }



			Comment comment = new Comment();
			comment.UserId = UserId;
			comment.PostId = PostId;
			comment.CommentContent = CommentContent;



			//檢查表單是否正確 /若正確則存入資料庫

			if (comment.CommentContent != null) {

                if (ModelState.IsValid)
                {
                    _context.Add(comment);
                    _context.SaveChanges();
                }

            }
           
        

            var data = _context.ViewPosts.Where(x => x.PostId == PostId).FirstOrDefault();

            return Json(data);
        }


        //########################################### 處理留言表單 ------------------------------






        //########################################### 點讚處理區 -----------------------------

        // POST: ThumbsTest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        public IActionResult Like(int? id)
        {

            //抓SESSION 中的 USERID
            var userid = HttpContext.Session.GetString("userID");

            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userid, out userIDnumber);



            //身分驗證---
            if (userid == null)
            {
                return RedirectToAction("Login", "Home_Login");
            }

            Thumb thumb = new Thumb();

            thumb.UserId = userIDnumber;
            thumb.PostId = id;

            var CheckLike = _context.Thumbs.Where(x => x.UserId == userIDnumber && x.PostId == thumb.PostId).FirstOrDefault();

            if (CheckLike == null)
            {



                if (ModelState.IsValid)
                {
                    _context.Add(thumb);
                    _context.SaveChanges();

                    var data = _context.ViewPostThumbCounts.Where(x => x.PostId == thumb.PostId).FirstOrDefault();

                    return Json(data);
                }

            }
            else
            {
                var data = _context.ViewPostThumbCounts.Where(x => x.PostId == thumb.PostId).FirstOrDefault();

                return Json(data);


            }

            return RedirectToAction("Post_Index", "Home_Post");
        }

        //########################################### 點讚處理區 ------------------------------











        //########################################### 文章管理 我的文章專區 ------------------------------

        public async Task<IActionResult> Post_UserArticle()  //--使用者自己寫的文章
        {


            //先抓使用者id
            var userIDs = HttpContext.Session.GetString("userID");

            //利用 Session id 來判別使用者是否登入 如果沒登入則導回登入頁


            if (userIDs == null)
            {
                return RedirectToAction("Login", "Home_Login");

            }


            //存使用者ID
            if (userIDs != null)
            {
                ViewBag.userIDindex = userIDs;
            }


            if (userIDs == null)
            {
                ViewBag.userIDindex = null;
            }


            //建立評論資料表
            var CommentData = _context.ViewPostComments.OrderByDescending(x => x.CommentTime).ToList();
            ViewBag.CommentData = CommentData;


            int userIDnumber;
            bool success = int.TryParse(userIDs, out userIDnumber);
            var projectDataBaseContext = _context.ViewPosts.Where(x => x.UserId == userIDnumber);



            return View(await projectDataBaseContext.OrderByDescending(x => x.PostId).ToListAsync());

        }


        //########################################### 文章管理 我的文章專區 ------------------------------







        //############################################文章編修處理部分--------------------------

        // GET: PostsTest/Edit/5
        public async Task<IActionResult> PostEdit(int? id)
        {

           

            var userIDs = HttpContext.Session.GetString("userID");

           //var userid = Convert.ToInt32(userIDs);
            ViewBag.userIDs = userIDs;

            


            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(int id, [Bind("PostId,UserId,PostCategory,PostTitle,PostPublishTime,PostContent,PostTag1,PostTag2,PostTag3,HiddenPost,PostrImgUrl")] Post post)
        {

            if (id != post.PostId)
            {
                return NotFound();
            }


            var userIDs = HttpContext.Session.GetString("userID");

            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userIDs, out userIDnumber);
            post.UserId = userIDnumber;


            if (ModelState.IsValid)
            {

                _context.Update(post);
                await _context.SaveChangesAsync();


                return RedirectToAction("Post_UserArticle", "Home_Post");
            }
            return View();
        }



        //############################################  文章編修處理部分------------------------








        //############################################  文章刪除處理部分-------------------
        //id 是postid 
        // GET: PostsTest/Delete/5

        //---------顯示要刪除的文章
        public async Task<IActionResult> PostDelete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }



        //刪除文章處理---------
        //id 是postid 
        // POST: PostsTest/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostDelete(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ProjectDataBaseContext.Posts'  is null.");
            }

            var post = await _context.Posts.FindAsync(id);

            if (post != null)
            {
                //刪除文章則用HiddenPost加註隱藏標記
                post.HiddenPost = "1";
                _context.Posts.Update(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Post_UserArticle", "Home_Post");
        }
        //############################################  文章刪除處理部分--------------------






        //############################################  文章收藏顯示部分處理部分--------------------

        public async Task<IActionResult> Post_UserReserve()
        {



            //身分驗證---沒登入不能看
            var userID = HttpContext.Session.GetString("userID");

            ViewBag.UserID = userID;

            //如果使用者未登入會被引導至登入頁面
            if (string.IsNullOrEmpty(userID))
            {

                return RedirectToAction("Login", "Home_Login");

            }






            //抓留言資料
            var CommentData = _context.ViewPostComments.OrderByDescending(x => x.CommentTime).ToList();
            ViewBag.CommentData = CommentData;

            var userIDindex = HttpContext.Session.GetString("userID");


            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userIDindex, out userIDnumber);

            int userids = userIDnumber;

            if (userIDindex != null)
            {
                ViewBag.userIDindex = userIDindex;
            }


            if (userIDindex == null)
            {
                ViewBag.userIDindex = null;
            }


            var projectDataBaseContext = _context.ViewBookUsers.Where(x => x.BookUser == userids);

            return View(await projectDataBaseContext.OrderByDescending(x => x.BookTime).ToListAsync());
        }

        //############################################  文章收藏顯示部分處理部分--------------------








        //############################################  文章收藏處理部分處理部分--------------------
        [HttpPost]
        public IActionResult Book(int? id)
        {
            var userid = HttpContext.Session.GetString("userID");


            //如果使用者未登入會被引導至登入頁面
            if (string.IsNullOrEmpty(userid))
            {

                return RedirectToAction("Login", "Home_Login");

            }




            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userid, out userIDnumber);


            Book book = new Book();
            book.UserId = userIDnumber;
            book.PostId = id;



            var checkbook = _context.Books.Where(x => x.UserId == userIDnumber && x.PostId == book.PostId).FirstOrDefault();

            if (ModelState.IsValid)
            {

                if (checkbook == null)
                {

                    ViewBag.book = 1;
                    _context.Add(book);
                    _context.SaveChanges();

                    var data = _context.ViewPostBookCounts.Where(x => x.PostId == book.PostId).FirstOrDefault();
                    return Json(data);

                }
                else
                {

                    ViewBag.book = 0;
                    var data = _context.ViewPostBookCounts.Where(x => x.PostId == book.PostId).FirstOrDefault();
                    return Json(data);


                }
            }

            return RedirectToAction("Book", "Home_Post");
        }



        //移除文章蒐藏
        [HttpPost, ActionName("CancelBook")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Book books)
        {

            var userid = HttpContext.Session.GetString("userID");


            //如果使用者未登入會被引導至登入頁面
            if (string.IsNullOrEmpty(userid))
            {

                return RedirectToAction("Login", "Home_Login");

            }






            var id = _context.Books.Where(x => x.UserId == books.UserId && x.PostId == books.PostId).Select(x => x.BookId).FirstOrDefault();

            if (_context.Books == null)
            {
                return Problem("Entity set 'ProjectDataBaseContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Post_UserReserve", "Home_Post");
        }


        //############################################  文章收藏處理部分處理部分--------------------









        //############################################  聊天室功能理部分處理部分--------------------


        //聊天室頁面顯示區塊處理
        public async Task<IActionResult> Post_Chat()
        {



            //匯入
            var userid = HttpContext.Session.GetString("userID");
            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userid, out userIDnumber);
            ViewBag.userIDindex = userIDnumber;






            if (string.IsNullOrEmpty(userid))
            {

                return RedirectToAction("Login", "Home_Login");

            }
            else
            {

                ViewChatRoom check = new ViewChatRoom();
                //檢查是否空值用
                check = (ViewChatRoom)_context.ViewChatRooms.Where(x => x.UserId == userIDnumber || x.PosterId == userIDnumber).FirstOrDefault();

                //用來填值用的
                int ChatRoomIds = Convert.ToInt32(TempData["TempChatRoomId"]);

                //檢查是否空值用 ，因為int 無法為null所以要先轉為文字才能比較
                string ChatRoomIdss = ChatRoomIds.ToString();






                //訊息資料表顯示資料部分-----------------------------------------------

                if (ChatRoomIdss != "0" && check != null)
                {

                    //抓文章標題
                    var ChatRoomPostTitle = _context.ViewChatRooms.Where(x => x.ChatRoomId == ChatRoomIds).FirstOrDefault().PostTitle.ToString();



                    //歷史聊天紀錄資料表---聊天紀錄要在修一下--  分成我問別人問題 及 別人問我問題


                    //我問別人問題的聊天紀錄-----
                    var ChatRoomHistory = _context.ViewChatRooms.Where(x => x.UserId == userIDnumber).OrderByDescending(x => x.ChatRoomTime).ToList();
                    ViewBag.ChatRoomHistory = ChatRoomHistory;



                    //別人問我問題的聊天紀錄---
                    var ChatRoomHistory2 = _context.ViewChatRooms.Where(x => x.PosterId == userIDnumber).OrderByDescending(x => x.ChatRoomTime).ToList();
                    ViewBag.ChatRoomHistory2 = ChatRoomHistory2;





                    var Messageview = _context.ViewChatRoomMessages.Where(x => x.ChatRoomId == ChatRoomIds).ToList();
                    ViewBag.Messageview = Messageview;
                    ViewBag.ChatRoomIds = ChatRoomIds;
                    ViewBag.ChatRoomPostTitle = ChatRoomPostTitle;

                }

                else
                {

                    ViewBag.Messageview = null;
                    ViewBag.ChatRoomIds = null;
                    ViewBag.ChatRoomIds = null;

                    TempData["emptyChatRoom"] = "(貼心提醒:您目前沒有任何聊天紀錄，請踴躍發問)";
                    return RedirectToAction("Post_Index", "Home_Post");
                }


                return View();

            }

        }


        //############################################  聊天室私聊處理部分--------------------


        public IActionResult blankChatRoom(int chatRoomId)
        {


            var userid = HttpContext.Session.GetString("userID");
            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userid, out userIDnumber);


            if (string.IsNullOrEmpty(userid))
            {

                return RedirectToAction("Login", "Home_Login");

            }
            else
            {

                var firstchatroomid = _context.ViewChatRoomMessages.Where(x => x.UserId == userIDnumber).OrderByDescending(x => x.MessageTime).Select(x => x.ChatRoomId).FirstOrDefault();


                TempData["TempChatRoomId"] = firstchatroomid;

                return RedirectToAction("Post_Chat", "Home_Post");

            }


        }





        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult HistoryChatRoomId(int chatRoomId)
        {

            TempData["TempChatRoomId"] = chatRoomId;
            return RedirectToAction("Post_Chat", "Home_Post");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetChat([Bind("ChatRoomId,UserId,ChatRoomTime,PostId")] ChatRoom chatRoom)
        {

            var userid = HttpContext.Session.GetString("userID");


            //強制換變數為數字變數
            int userIDnumber;
            bool success = int.TryParse(userid, out userIDnumber);



            if (string.IsNullOrEmpty(userid))
            {

                return RedirectToAction("Login", "Home_Login");

            }

            else
            {

                //此段有問題 --容易造成系統 ChatroomID重複因為資料結構中int不能是null
                ChatRoom chatRoom1 = new ChatRoom();


                // var checkChatroomID = _context.ChatRooms.Where(x => x.UserId == chatRoom.UserId && x.PostId == chatRoom.PostId).FirstOrDefault().ChatRoomId.ToString();

                //string checkChatroomIDTEST= checkChatroomID.ToString();

                chatRoom1 = _context.ChatRooms.Where(x => x.UserId == chatRoom.UserId && x.PostId == chatRoom.PostId).FirstOrDefault();

                if (chatRoom1 == null)
                {


                    TempData["chatRoomUserid"] = chatRoom.UserId;
                    TempData["chatRoomPostid"] = chatRoom.PostId;



                    if (ModelState.IsValid)
                    {
                        _context.Add(chatRoom);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("CreatMessage", "Home_Post");
                    }

                }
                else
                {
                    TempData["chatRoomIdExist"] = "YES";
                    int ChatRoomId = _context.ChatRooms.Where(x => x.UserId == chatRoom.UserId && x.PostId == chatRoom.PostId).FirstOrDefault().ChatRoomId;

                    TempData["TempChatRoomId"] = chatRoom1.ChatRoomId;

                    return RedirectToAction("Post_Chat", "Home_Post");
                }

                return RedirectToAction("Post_Index", "Home_Post");

            }

        }



        //############################################  聊天室私聊處理部分--------------------





        //############################################  聊天室私聊自動創建處理部分--------------------

        public async Task<IActionResult> CreatMessage(Messagess messagess)
        {

            int chatRoomUserid = Convert.ToInt32(TempData["chatRoomUserid"]);
            int chatRoomPostid = Convert.ToInt32(TempData["chatRoomPostid"]);

            int ChatRoomId = _context.ChatRooms.Where(x => x.UserId == chatRoomUserid && x.PostId == chatRoomPostid).FirstOrDefault().ChatRoomId;
            int UserId = (int)_context.Posts.Where(x => x.PostId == chatRoomPostid).FirstOrDefault().UserId;
            string postitle = (string)_context.Posts.Where(x => x.PostId == chatRoomPostid).FirstOrDefault().PostTitle;


            messagess.MessageContent = "任何問題歡迎留言";
            messagess.UserId = UserId;
            messagess.ChatRoomId = ChatRoomId;

            if (ModelState.IsValid)
            {
                _context.Messagesses.Add(messagess);
                await _context.SaveChangesAsync();

                TempData["TempChatRoomId"] = messagess.ChatRoomId;
                TempData["TempChatRoomPostTitle"] = postitle;

                return RedirectToAction("Post_Chat", "Home_Post");
            }

            return RedirectToAction("Post_Chat", "Home_Post");
        }


        //############################################  聊天室私聊自動創建處理部分--------------------









        //############################################  聊天室送出留言處理處理部分--------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> creatNewMessage([Bind("MessageId,ChatRoomId,UserId,MessageContent,MessageTime")] Messagess messagess)
        {
            if (ModelState.IsValid)
            {
                _context.Add(messagess);
                await _context.SaveChangesAsync();


                TempData["TempChatRoomPostTitle"] = _context.ViewChatRoomMessages.Where(x => x.UserId == messagess.UserId && x.ChatRoomId == messagess.ChatRoomId).FirstOrDefault().PostTitle;
                TempData["TempChatRoomId"] = messagess.ChatRoomId;
                return RedirectToAction("Post_Chat", "Home_Post");
            }

            return RedirectToAction("Post_Chat", "Home_Post");
        }

        //############################################  聊天室送出留言處理處理部分--------------------








    }
}

