using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticUtility.SuperAdminEndUser + ", " + StaticUtility.AdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private int PageSize = 5;
        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int productPage, string searchName, string searchEmail, string searchPhone, string searchDate)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVM  = new AppointmentViewModel()
            {
               Appointments = new List<Models.Appointments>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Admin/Appointments?productsPage=:");
            param.Append("&searchName");

            if(searchName != null)
            {
                param.Append(searchName);
            }
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            if (searchDate != null)
            {
                param.Append(searchDate);
            }

            appointmentVM.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();
            if (searchName != null) 
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }
            }

            if(searchName == null)
            {
                appointmentVM.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();
            }

            var count = appointmentVM.Appointments.Count;

            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(p => p.AppointmentDate)
                .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();

            appointmentVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            return View(appointmentVM);
        }

        //GET EDIT 
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = (IEnumerable<Products>)( from p in _db.Products
                                                    join a in _db.ProductsSelectedForAppointments
                                                    on p.Id equals a.ProductId
                                                    where a.AppointmentId == id
                                                    select p).Include("ProductTypes");

            AppointmentDetailsViewModel objAppVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUsers.ToList(),
                Products = products.ToList()
            };

            return View(objAppVM);
        }
        //POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppointmentDetailsViewModel objAppointmentVM)
        {
            if (ModelState.IsValid)
            {
                objAppointmentVM.Appointment.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate
                                    .AddHours(objAppointmentVM.Appointment.AppointmentTime.Hour)
                                    .AddMinutes(objAppointmentVM.Appointment.AppointmentTime.Minute);

                var appointmentFromDb = _db.Appointments.Where(a => a.Id == objAppointmentVM.Appointment.Id).FirstOrDefault();

                appointmentFromDb.CustomerName = objAppointmentVM.Appointment.CustomerName;
                appointmentFromDb.CustomerEmail = objAppointmentVM.Appointment.CustomerEmail;
                appointmentFromDb.CustomerPhoneNumber = objAppointmentVM.Appointment.CustomerPhoneNumber;
                appointmentFromDb.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate;
                appointmentFromDb.IsConfirmed = objAppointmentVM.Appointment.IsConfirmed;
                if (User.IsInRole(StaticUtility.SuperAdminEndUser))
                {
                    appointmentFromDb.SalesPersonId = objAppointmentVM.Appointment.SalesPersonId;
                }
                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(objAppointmentVM);
        }
       
        //Details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointments
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUsers.ToList(),
                Products =  productList.ToList()
            };

            return View(objAppointmentVM);
        }

        //GET Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointments
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUsers.ToList(),
                Products = productList.ToList()
            };

            return View(objAppointmentVM);
        }
        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    } 
}


//if (User.IsInRole(StaticUtility.AdminEndUser) || User.IsInRole(StaticUtility.SuperAdminEndUser))
//{
//    appointmentVM.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();
//    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
//}