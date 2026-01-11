using Microsoft.AspNetCore.Mvc;
using ProjektSezon2.Models;
using System.Diagnostics;

namespace ProjektSezon2.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
