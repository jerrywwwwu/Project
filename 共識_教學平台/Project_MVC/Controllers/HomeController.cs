using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Project_MVC.Models;

namespace Project_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProjectDataBaseContext _context;

        public HomeController (ILogger<HomeController> logger, ProjectDataBaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Home_Leader ()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Home_Index ()
        {
            // 使用 Tuple<> => 可以將多個 Model 和在一起做使用
            // A.抓取Model 清單 資料
            IEnumerable<User> userList = _context.Users.ToList ();
            IEnumerable<Post> postsList = _context.Posts.ToList ();
            IEnumerable<ClassCourse> classCoursesList = _context.ClassCourses.ToList();
            // 案讚數多到少排列，取前面4個
            IEnumerable<ViewPost> viewPosts = _context.ViewPosts.OrderByDescending(p=>p.PostThumbCount).Take(4);

            // B.用 Tuple 把 多個 Model 裝起來
            var tupleData = new Tuple<IEnumerable<User>, IEnumerable<Post>, IEnumerable<ClassCourse>, IEnumerable<ViewPost>>(userList, postsList, classCoursesList, viewPosts);

            return View(tupleData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}