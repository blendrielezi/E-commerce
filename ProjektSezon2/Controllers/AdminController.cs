using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektSezon2.Filters;
using ProjektSezon2.Models;
using System.Diagnostics;

namespace ProjektSezon2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private const string SpecialAdminId = "YOUR_SPECIAL_ADMIN_ID";

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        [ServiceFilter(typeof(AdminFilter))]
        public IActionResult MainAdmin()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
