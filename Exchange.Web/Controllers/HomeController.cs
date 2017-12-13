using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Exchange.Services;
using Exchange.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
