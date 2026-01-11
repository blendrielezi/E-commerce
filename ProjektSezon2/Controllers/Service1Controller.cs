using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjektSezon2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Service1Controller : Controller
    {
        public IActionResult Service(int id)
        {
            // Krijon model që përmban ID e kategoris përdoret n view
            var model = new { CategoryId = id };

            return View(model);
        }
        public IActionResult EditService()
        {
            return View();
        }
        public IActionResult Create(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            return View();
        }



    }
}
