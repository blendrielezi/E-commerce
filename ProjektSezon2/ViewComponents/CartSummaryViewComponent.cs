// ViewComponents/CartSummaryViewComponent.cs
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Models;

namespace ProjektSezon2.ViewComponents
{
    // ViewModel for the Cart summary
    public class CartSummaryViewModel
    {
        public bool IsAuthenticated { get; set; }
        public int ItemCount { get; set; }
    }

    // ViewComponent to render the cart icon + count
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartSummaryViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View(new CartSummaryViewModel { IsAuthenticated = false, ItemCount = 0 });
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return View(new CartSummaryViewModel { IsAuthenticated = false, ItemCount = 0 });
            }

            // Find the active (unpaid) order
            var activeOrder = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.ApplicationUserId == user.Id && o.PaymentStatus == null);

            var count = activeOrder?.Items.Sum(i => i.Quantity ?? 0) ?? 0;

            return View(new CartSummaryViewModel { IsAuthenticated = true, ItemCount = count });
        }
    }
}
