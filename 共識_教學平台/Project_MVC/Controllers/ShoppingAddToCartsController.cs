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
    public class ShoppingAddToCartsController : Controller
    {
        private readonly ProjectDataBaseContext _context;

        public ShoppingAddToCartsController(ProjectDataBaseContext context)
        {
            _context = context;
        }

//購物車頁面：使用者可以查看自己購物車有的課程名稱等資料
        public IActionResult ShoppingCart() {

            //如果使用者沒有登入但是想看購物車(隸屬於會員專區)，會被引導到登入頁面
            var id = HttpContext.Session.GetString("userID");
            bool isParsed = int.TryParse(id, out int number);

            ViewBag.userid = number;

            //開始判斷
            if (string.IsNullOrEmpty(id)) {
                //重新導向的功能，我要讓使用者登入之後記得回家，也就是回到購物車。
                TempData["ReturnUrl"] = "/ShoppingAddToCarts/ShoppingCart";
                string userName = HttpContext.Session.GetString("userName") ?? "Guest";
                if (userName == "Guest") {
                    return Redirect("/Home_Login/Login");
                }
                return RedirectToAction("Login", "Home_Login");
            }

            var CoursesInCart2 = _context.ViewBuiedCoursesNames.Where(x => x.UserId == number);

            var CoursesInCart = from data in _context.ViewBuiedCoursesNames
                                select new ViewBuiedCoursesName
                                {
                                    UserId = data.UserId,
                                    UserName = data.UserName,
                                    CourseName = data.CourseName,
                                    CoursePrice = data.CoursePrice,
                                    ShoppingCartId = data.ShoppingCartId,
                                };

            return View(CoursesInCart2.ToList());
        }

//購物車頁面：使用者可以查看自己購物車有的課程名稱等資料，這裡可以刪除購物車裡面不要的課程
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShoppingCart(ShoppingAddToCart data) {

            var DeleteUnlikeCourse = await _context.ShoppingAddToCarts
                    .FirstOrDefaultAsync(item =>
                    item.ShoppingCartId == data.ShoppingCartId &&
                    item.UserId == data.UserId &&
                    item.CourseId == data.CourseId
                    );

            if (DeleteUnlikeCourse != null) {
                _context.ShoppingAddToCarts.Remove(DeleteUnlikeCourse);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ShoppingCart");
        }

//===========================================================
//結帳頁面：使用者可以查看自己購物車有的課程名稱等資料，這裡已經不行刪除不要的課程
        public IActionResult ShoppingCheckout() {
            //如果使用者沒有登入但是想看購物車(隸屬於會員專區)，會被引導到登入頁面
            var id = HttpContext.Session.GetString("userID");
            bool isParsed = int.TryParse(id, out int number);

            //開始判斷
            if (string.IsNullOrEmpty(id)) {
                //這是我要重新導向的功能，我要讓使用者登入之後記得回家，回到購物車
                TempData["ReturnUrl"] = "/ShoppingAddToCarts/ShoppingCart";
                string userName = HttpContext.Session.GetString("userName") ?? "Guest";
                if (userName == "Guest") {
                    return Redirect("/Home_Login/Login");
                }
                return RedirectToAction("Login", "Home_Login");
            }
            var CoursesInCart2 = _context.ViewBuiedCoursesNames.Where(x => x.UserId == number);

            var CoursesInCart = from data in _context.ViewBuiedCoursesNames
                                select new ViewBuiedCoursesName
                                {
                                    UserId = data.UserId,
                                    UserName = data.UserName,
                                    CourseName = data.CourseName,
                                    CoursePrice = data.CoursePrice,
                                    ShoppingCartId = data.ShoppingCartId,
                                };
            return View(CoursesInCart2.ToList());
        }

        //結帳頁面：使用者可以查看自己購物車有的課程名稱等資料，這裡已經不行刪除不要的課程，接著送出訂單後，後台會新增一筆訂單資料
        [HttpPost]
        public async Task<IActionResult> ShoppingCheckout (IFormCollection OrderForm, List<int> PikachuCat, List<decimal> PikachuDog)
        {
            if (OrderForm != null)
            {
                var id = HttpContext.Session.GetString("userID");
                bool isParsed = int.TryParse(id, out int UserIds);

                var CartCheckout = new ShoppingOrder();
                CartCheckout.UserId = UserIds;
                CartCheckout.TotalAmount = Convert.ToInt32(OrderForm["HiddentotalPriceSumCheck"]);
                CartCheckout.OrderDate = DateTime.Now;
                _context.Add(CartCheckout);
                await _context.SaveChangesAsync();


                int minCount = Math.Min(PikachuCat.Count, PikachuDog.Count);
                for (int i = 0; i < minCount; i++)
                {
                    var BeeOrange = new ShoppingOrderDetail();
                    BeeOrange.OrderId = CartCheckout.OrderId;
                    //BeeOrange.OrderId = 82;
                    BeeOrange.CourseId = PikachuCat[i];
                    BeeOrange.Price = Convert.ToInt32(PikachuDog[i]);
                    _context.Add(BeeOrange);
                    await _context.SaveChangesAsync();
                }

                //var ClearCart = _context.ShoppingAddToCarts.Where(x=>x.UserId == UserIds).Find();


                //結帳完之後要清空購物車
                var userCartItems = _context.ShoppingAddToCarts.Where(x => x.UserId == UserIds).ToList();

                if (userCartItems.Count > 0)
                {
                    _context.ShoppingAddToCarts.RemoveRange(userCartItems);
                    await _context.SaveChangesAsync();
                }



                return RedirectToAction("Index", "Home_Login");
            }
            return RedirectToAction("Index", "Home_Login");
        }





        // ==================================================================
        //這是加入購物車的那個行為，加入後，"購物區加入購物車"資料表，就會多一筆訂單
        public IActionResult AddToCart ()
        {
            //只能看到自己的
            var id = HttpContext.Session.GetString("userID");
            bool isParsed = int.TryParse(id, out int number);

            //開始判斷如果使用者沒有登入但是想加入購物車(隸屬於會員專區)，會被引導到登入頁面
            if (string.IsNullOrEmpty(id))
            {
                //重新導向的功能，我要讓使用者登入之後記得回家，也就是回到課程瀏覽頁面(ShoppingCourseBrowse)。
                TempData["ReturnUrl"] = "/Home_Shopping/Shopping_Index";
                string userName = HttpContext.Session.GetString("userName") ?? "Guest";
                if (userName == "Guest")
                {
                    return Redirect("/Home_Login/Login");
                }
                return RedirectToAction("Shopping_Index", "Home_Shopping");
            }

            int s = Convert.ToInt32(HttpContext.Request.Query["CID"]);
            //var egg = _context.ClassCourses.Where(x => x.CourseId == s).Select(j => j);

            bool isOrderExixts = _context.ShoppingAddToCarts.Any(cart => cart.UserId == number && cart.CourseId == s);

            //拿來防堵使用者重複加入購物車的機制
            if (isOrderExixts)
            {
                //如果這門課已經加入購物車，用sweetalert提醒使用者不要再加了
                TempData["DuplicateOrder"] = true;
            }
            else
            {
                //如果還沒購買，就丟到購物車沒問題
                var applecat = new ShoppingAddToCart();
                //applecat.UserId = 2; //先寫死
                applecat.UserId = number;
                applecat.CourseId = s;
                _context.Add(applecat);
                _context.SaveChanges();
                TempData["AddedToCart"] = true;
            }
            return RedirectToAction("Shopping_Index", "Home_Shopping");
        }

        // 這是測試用的
        // 測試把訂單明細資料表叫出來
        public IActionResult OrderDetailsTest ()
        {
            //var id = HttpContext.Session.GetString("userID");
            //bool isParsed = int.TryParse(id, out int number);

            var seeorderdetail = _context.ShoppingOrderDetails.Where(i => i.OrderId == 69); //先寫死
            ViewBag.SeeMyCourses = seeorderdetail.ToList();
            return View();
        }






        // GET: ShoppingAddToCarts
        public async Task<IActionResult> Index()
        {
			var projectDataBaseContext = _context.ShoppingAddToCarts.Include(s => s.Course).Include(s => s.User);
            return View(await projectDataBaseContext.ToListAsync());
        }

        // GET: ShoppingAddToCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ShoppingAddToCarts == null)
            {
                return NotFound();
            }

            var shoppingAddToCart = await _context.ShoppingAddToCarts
                .Include(s => s.Course)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.ShoppingCartId == id);
            if (shoppingAddToCart == null)
            {
                return NotFound();
            }

            return View(shoppingAddToCart);
        }

        // GET: ShoppingAddToCarts/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.ClassCourses, "CourseId", "CourseId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: ShoppingAddToCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShoppingCartId,UserId,CourseId,CartItemPrice")] ShoppingAddToCart shoppingAddToCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingAddToCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.ClassCourses, "CourseId", "CourseId", shoppingAddToCart.CourseId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", shoppingAddToCart.UserId);
            return View(shoppingAddToCart);
        }

        // GET: ShoppingAddToCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ShoppingAddToCarts == null)
            {
                return NotFound();
            }

            var shoppingAddToCart = await _context.ShoppingAddToCarts.FindAsync(id);
            if (shoppingAddToCart == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.ClassCourses, "CourseId", "CourseId", shoppingAddToCart.CourseId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", shoppingAddToCart.UserId);
            return View(shoppingAddToCart);
        }

        // POST: ShoppingAddToCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShoppingCartId,UserId,CourseId,CartItemPrice")] ShoppingAddToCart shoppingAddToCart)
        {
            if (id != shoppingAddToCart.ShoppingCartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingAddToCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingAddToCartExists(shoppingAddToCart.ShoppingCartId))
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
            ViewData["CourseId"] = new SelectList(_context.ClassCourses, "CourseId", "CourseId", shoppingAddToCart.CourseId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", shoppingAddToCart.UserId);
            return View(shoppingAddToCart);
        }

        // GET: ShoppingAddToCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ShoppingAddToCarts == null)
            {
                return NotFound();
            }

            var shoppingAddToCart = await _context.ShoppingAddToCarts
                .Include(s => s.Course)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.ShoppingCartId == id);
            if (shoppingAddToCart == null)
            {
                return NotFound();
            }

            return View(shoppingAddToCart);
        }

        // POST: ShoppingAddToCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ShoppingAddToCarts == null)
            {
                return Problem("Entity set 'ProjectDataBaseContext.ShoppingAddToCarts'  is null.");
            }
            var shoppingAddToCart = await _context.ShoppingAddToCarts.FindAsync(id);
            if (shoppingAddToCart != null)
            {
                _context.ShoppingAddToCarts.Remove(shoppingAddToCart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingAddToCartExists(int id)
        {
          return (_context.ShoppingAddToCarts?.Any(e => e.ShoppingCartId == id)).GetValueOrDefault();
        }
    }
}
