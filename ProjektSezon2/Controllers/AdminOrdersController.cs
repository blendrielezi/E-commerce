// Controllers/AdminOrdersController.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Filters;

namespace ProjektSezon2.Controllers
{
    [Authorize(Roles = "Admin")]
    [CustomExceptionFilter]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminOrdersController(ApplicationDbContext db) => _db = db;

        // GET: /AdminOrders/Paid
        public async Task<IActionResult> Paid()
        {
            var paid = await _db.Orders
                .Where(o => o.PaymentStatus == "Completed")
                .Include(o => o.ApplicationUser)
                .ToListAsync();
            return View(paid);
        }

        // GET: /AdminOrders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                .Where(o => o.Id == id)
                .Include(o => o.ApplicationUser)
                .Include(o => o.Items).ThenInclude(i => i.Service)
                .FirstOrDefaultAsync();
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: /AdminOrders/Ship/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Ship(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order != null && order.PaymentStatus == "Completed")
            {
                order.PaymentStatus = "Shipped";
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Paid));
        }

        // GET: /AdminOrders/Shipped
        public async Task<IActionResult> Shipped()
        {
            var shipped = await _db.Orders
                .Where(o => o.PaymentStatus == "Shipped")
                .Include(o => o.ApplicationUser)
                .ToListAsync();
            return View(shipped);
        }

        // GET: /AdminOrders/ShippedDetails/5
        public async Task<IActionResult> ShippedDetails(int id)
        {
            var order = await _db.Orders
                .Where(o => o.Id == id && o.PaymentStatus == "Shipped")
                .Include(o => o.ApplicationUser)
                .Include(o => o.Items).ThenInclude(i => i.Service)
                .FirstOrDefaultAsync();
            if (order == null) return NotFound();
            return View("Details", order); 
        }
        // GET: /AdminOrders/TestException
        public IActionResult TestException()
        {
            throw new ArgumentOutOfRangeException("TestGabim", "Test për exception filter");
        }

    }
}
