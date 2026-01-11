// Controllers/PaymentController.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjektSezon2.Data;
using ProjektSezon2.Models;

namespace ProjektSezon2.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public PaymentController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // GET: /Payment/Checkout,i kalon ne view per te filluat pagesne
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var order = await _db.Orders
                .Include(o => o.Items).
                    ThenInclude(i => i.Service)
                .FirstOrDefaultAsync(o => o.ApplicationUserId == userId && o.PaymentStatus == null);

            if (order == null || order.Items == null || !order.Items.Any())
                return RedirectToAction("MyCart", "Cart");

            var total = order.Items.Sum(i => (i.Quantity ?? 0) * (i.UnitPrice ?? 0m));

            ViewBag.BusinessEmail = _config["PayPal:BusinessEmail"];
            ViewBag.ReturnUrl = _config["PayPal:ReturnUrl"];
            ViewBag.CancelUrl = _config["PayPal:CancelUrl"];
            ViewBag.Amount = total.ToString("F2");
            ViewBag.Invoice = order.Id;
            ViewBag.Currency = "EUR";

            return View(order);
        }

        // GET: /Payment/Success?invoice=1
        [AllowAnonymous]
        public async Task<IActionResult> Success(int invoice)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == invoice);

            if (order != null)
            {
                order.TotalAmount = order.Items?
                    .Sum(i => (i.Quantity ?? 0) * (i.UnitPrice ?? 0m))
                    ?? 0m;
                order.PaymentStatus = "Completed";

                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
            }

            return View();
        }

        // GET: /Payment/Cancel
        [AllowAnonymous]
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
