// Controllers/ReportController.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;

namespace ProjektSezon2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReportController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var sixMonthsAgo = now.AddMonths(-5);
            var weekAgo = now.Date.AddDays(-6);

            // 1) Sales by month (last 6 months)
            var salesByMonth = await _db.Orders
                .Where(o => o.CreatedAt >= sixMonthsAgo && o.PaymentStatus != null)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new { Month = g.Key.Month + "/" + g.Key.Year, Total = g.Sum(o => o.TotalAmount) ?? 0m })
                .ToListAsync();

            // 2) Top 5 services by quantity sold
            var topServices = await _db.OrderItems
                .Include(i => i.Service)
                .GroupBy(i => i.Service.Name)
                .Select(g => new { Service = g.Key, Quantity = g.Sum(i => i.Quantity) ?? 0 })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToListAsync();

            // 3) Sessions per day (last 7 days)
            var sessionsByDay = await _db.Sessions
                .Where(s => s.StartedAt >= weekAgo)
                .GroupBy(s => s.StartedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key.ToString("MM/dd"), Count = g.Count() })
                .ToListAsync();

            // 4) parapelqimi per cookie
            var totalConsents = await _db.CookieConsents.CountAsync();
            var analyticsCount = await _db.CookieConsents.CountAsync(c => c.Analytics == true);
            var marketingCount = await _db.CookieConsents.CountAsync(c => c.Marketing == true);

            // 5) Services per category
            var servicesPerCategory = await _db.Services
                .Include(s => s.Category)
                .GroupBy(s => s.Category != null ? s.Category.Name : "Uncategorized")
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            // 6) Total counts: users, orders, services, categories
            var totalUsers = await _db.Users.CountAsync();
            var totalOrders = await _db.Orders.CountAsync();
            var totalServices = await _db.Services.CountAsync();
            var totalCategories = await _db.Categories.CountAsync();

            // 7) Orders per month (last 6 months)
            var ordersByMonth = await _db.Orders
                .Where(o => o.CreatedAt >= sixMonthsAgo)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new { Month = g.Key.Month + "/" + g.Key.Year, Count = g.Count() })
                .ToListAsync();

            // 8) Average order value per month
            var avgOrderValue = salesByMonth
                .Select(s => new { s.Month, Avg = s.Total / (ordersByMonth.FirstOrDefault(o => o.Month == s.Month)?.Count ?? 1m) })
                .ToList();

            // Pass data to ViewBag
            ViewBag.SalesLabels = salesByMonth.Select(x => x.Month).ToArray();
            ViewBag.SalesData = salesByMonth.Select(x => x.Total).ToArray();
            ViewBag.TopServicesLabels = topServices.Select(x => x.Service).ToArray();
            ViewBag.TopServicesData = topServices.Select(x => x.Quantity).ToArray();
            ViewBag.SessionLabels = sessionsByDay.Select(x => x.Date).ToArray();
            ViewBag.SessionData = sessionsByDay.Select(x => x.Count).ToArray();
            ViewBag.CookieLabels = new[] { "Analytics", "Marketing", "Total" };
            ViewBag.CookieData = new object[] { analyticsCount, marketingCount, totalConsents };
            ViewBag.ServicesCatLabels = servicesPerCategory.Select(x => x.Category).ToArray();
            ViewBag.ServicesCatData = servicesPerCategory.Select(x => x.Count).ToArray();
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalServices = totalServices;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.OrderLabels = ordersByMonth.Select(x => x.Month).ToArray();
            ViewBag.OrderData = ordersByMonth.Select(x => x.Count).ToArray();
            ViewBag.AvgLabels = avgOrderValue.Select(x => x.Month).ToArray();
            ViewBag.AvgData = avgOrderValue.Select(x => x.Avg).ToArray();

            return View();
        }
    }
}