using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models.ViewModel
{
    [Authorize(Roles = StaticUtility.SuperAdminEndUser + "," + StaticUtility.AdminEndUser )]
    [Area("Admin")]
    public class AppointmentViewModel
    {
        public List<Appointments> Appointments { get; set; }
    }
}
