using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticUtility.SuperAdminEndUser + ", " + StaticUtility.AdminEndUser)]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList());
        }
        //GET EDIT 
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }
        //POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
                userFromDb.Name = applicationUser.Name;
                userFromDb.PhoneNumber = applicationUser.PhoneNumber;
                userFromDb.Email = applicationUser.Email;
                userFromDb.IsAdmin = applicationUser.IsAdmin;

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(applicationUser);

        }

        //GET DELETE 
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }
        //POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
            //userFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            _db.Remove(userFromDb);

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //GET DISABLE 
        public async Task<IActionResult> Disable(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }
        //POST DISABLE
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public IActionResult DisableConfirmed(string id)
        {
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
            userFromDb.LockoutEnd = DateTime.Now.AddMinutes(10);

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //GET ENABLE 
        public async Task<IActionResult> Enable(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }
        //POST DISABLE
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public IActionResult EnableConfirmed(string id)
        {
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
            userFromDb.LockoutEnd = null;

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}