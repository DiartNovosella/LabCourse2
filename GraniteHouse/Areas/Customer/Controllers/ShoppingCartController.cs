using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        //GET INDEX SHOPPING CART
        public async Task<IActionResult> Index()
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart.Count > 0)
            {
                foreach (int cartItem in lstShoppingCart)
                {
                    Products prod = _db.Products.Include(p => p.SpecialTags).Include(p => p.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefault();
                    ShoppingCartVM.Products.Add(prod);
                }
            }
            return View(ShoppingCartVM);
        }
        //GET INDEX SHOPPING CART
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate
                                                            .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour)
                                                            .AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);

            Appointments appointments = ShoppingCartVM.Appointments;
            _db.Appointments.Add(appointments);
            _db.SaveChanges();

            int appointmentId = appointments.Id;

            foreach (int productId in lstCartItems)
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };
                _db.ProductsSelectedForAppointments.Add(productsSelectedForAppointment);

            }
            _db.SaveChanges();
            lstCartItems = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            return RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId });
        }

        //REMOVE FROM SHOPPING CART SESSION
        public IActionResult Remove(int id)
        {
            List<int> listCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (listCartItems.Count > 0)
            {
                if (listCartItems.Contains(id))
                {
                    listCartItems.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", listCartItems);
            return RedirectToAction(nameof(Index));
        }

        //GET APPOINTMENT CONFIRMATION
        public IActionResult AppointmentConfirmation(int id)
        {
            ShoppingCartVM.Appointments = _db.Appointments.Where(a => a.Id == id).FirstOrDefault();

            List<ProductsSelectedForAppointment> objProdList = _db.ProductsSelectedForAppointments.Where(p => p.AppointmentId == id).ToList();

            foreach (ProductsSelectedForAppointment prodAptObj in objProdList)
            {
                ShoppingCartVM.Products.Add(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault());
            }

            return View(ShoppingCartVM);
        }
    }
}