using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjektSezon2.Data;
using Stripe.Checkout;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektSezon2.Controllers
{
    [Authorize]
    
    public class PaymentstripeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public PaymentstripeController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
            Stripe.StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }
        //Krijon sesion pagese me detajet e porosis perdoruesit dhe e ridrejton faqen e pagess
   
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var order = await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Service)
                .FirstOrDefaultAsync(o => o.ApplicationUserId == userId && o.PaymentStatus == null);

            if (order == null || !order.Items.Any())
                return RedirectToAction("MyCart", "Cart");

            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = order.Items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)((item.UnitPrice ?? 0) * 100),
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Service.Name
                        },
                    },
                    Quantity = item.Quantity ?? 1,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"{domain}/Paymentstripe/Success?orderId={order.Id}",
                CancelUrl = $"{domain}/Paymentstripe/Cancel"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url); 
        }

        [AllowAnonymous]
        
        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                order.PaymentStatus = "Completed";
                order.TotalAmount = order.Items.Sum(i => (i.Quantity ?? 0) * (i.UnitPrice ?? 0));
                await _db.SaveChangesAsync();
            }

            return View();
        }

        [AllowAnonymous]
        
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
