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
    public class UserTeachersTestController : Controller
    {
        private readonly ProjectDataBaseContext _context;

        public UserTeachersTestController(ProjectDataBaseContext context)
        {
            _context = context;
        }

        // GET: UserTeachersTest
        public async Task<IActionResult> Index()
        {
            var projectDataBaseContext = _context.UserTeachers.Include(u => u.User);
            return View(await projectDataBaseContext.ToListAsync());
        }

        // GET: UserTeachersTest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserTeachers == null)
            {
                return NotFound();
            }

            var userTeacher = await _context.UserTeachers
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (userTeacher == null)
            {
                return NotFound();
            }

            return View(userTeacher);
        }

        // GET: UserTeachersTest/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserEmail");
            return View();
        }

        // POST: UserTeachersTest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeacherId,UserId,UserIntro1,UserIntro2,UserBgimage")] UserTeacher userTeacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userTeacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserEmail", userTeacher.UserId);
            return View(userTeacher);
        }

        // GET: UserTeachersTest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserTeachers == null)
            {
                return NotFound();
            }

            var userTeacher = await _context.UserTeachers.FindAsync(id);
            if (userTeacher == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserEmail", userTeacher.UserId);
            return View(userTeacher);
        }

        // POST: UserTeachersTest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeacherId,UserId,UserIntro1,UserIntro2,UserBgimage")] UserTeacher userTeacher)
        {
            if (id != userTeacher.TeacherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userTeacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserTeacherExists(userTeacher.TeacherId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserEmail", userTeacher.UserId);
            return View(userTeacher);
        }

        // GET: UserTeachersTest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserTeachers == null)
            {
                return NotFound();
            }

            var userTeacher = await _context.UserTeachers
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (userTeacher == null)
            {
                return NotFound();
            }

            return View(userTeacher);
        }

        // POST: UserTeachersTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserTeachers == null)
            {
                return Problem("Entity set 'ProjectDataBaseContext.UserTeachers'  is null.");
            }
            var userTeacher = await _context.UserTeachers.FindAsync(id);
            if (userTeacher != null)
            {
                _context.UserTeachers.Remove(userTeacher);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserTeacherExists(int id)
        {
          return (_context.UserTeachers?.Any(e => e.TeacherId == id)).GetValueOrDefault();
        }
    }
}
