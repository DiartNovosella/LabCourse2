using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraniteHouse.Models;
using GraniteHouse.Data;
using Microsoft.EntityFrameworkCore;
using GraniteHouse.Extensions;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;

namespace GraniteHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(string searchProduct)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var productList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();

            if (searchProduct != null)
            {
                productList = productList.Where(a => a.Name.ToLower().Contains(searchProduct.ToLower())).ToList();
            }
           
            return View(productList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m => m.Id == id).FirstOrDefaultAsync();
            return View(product);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost(int id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(listShoppingCart == null)
            {
                listShoppingCart = new List<int>();
            }
            listShoppingCart.Add(id);
            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);
            return RedirectToAction("Index","Home", new { area = "Customer"});
        }

        public IActionResult Remove(int id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (listShoppingCart.Count > 0)
            {
                if (listShoppingCart.Contains(id))
                {
                    listShoppingCart.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //consume list from webServices
        //    List<Images> emp = new List<Images>();
        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var response = await httpClient.GetAsync("https://localhost:44371/api/images"))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //emp = JsonConvert.DeserializeObject<List<Images>>(apiResponse);
        //        }
        //    }
    }
}
