using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServiceAds.Models;

namespace GraniteHouse.Models.ViewModel
{
    public class HomeViewModel
    {
        public List<Products> Products { get; set; }
        public List<Images> Images { get; set; }
    }
}
