// Controllers/CartController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using ProjektSezon2.Extensions;
using System.Security.Claims;

namespace ProjektSezon2.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyCart()
        {
            var vm = new CartIndexViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var order = await _db.Orders
                        .Include(o => o.Items)
                        .ThenInclude(i => i.Service)
                        .FirstOrDefaultAsync(o => o.ApplicationUserId == user.Id && o.PaymentStatus == null);

                    if (order != null)
                    {
                        vm.Items = order.Items.Select(i => new CartItemViewModel
                        {
                            OrderItemId = i.Id,
                            ServiceId = i.ServiceId ?? 0,
                            Name = i.Service.Name,
                            UnitPrice = i.UnitPrice ?? 0m,
                            Quantity = i.Quantity ?? 0,
                            Subtotal = (i.UnitPrice ?? 0m) * (i.Quantity ?? 0)
                        }).ToList();

                        vm.Total = vm.Items.Sum(x => x.Subtotal);
                    }
                }
            }
            else
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItemSession>>("Cart") ?? new();
                vm.Items = cart.Select(i => new CartItemViewModel
                {
                    ServiceId = i.ServiceId,
                    Name = i.Name,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.UnitPrice * i.Quantity
                }).ToList();

                vm.Total = vm.Items.Sum(x => x.Subtotal);
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id, string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var order = await _db.Orders
                    .FirstOrDefaultAsync(o => o.ApplicationUserId == user.Id && o.PaymentStatus == null)
                    ?? new Order { ApplicationUserId = user.Id, CreatedAt = DateTime.UtcNow, PaymentStatus = null };

                if (order.Id == 0)
                    _db.Orders.Add(order);

                await _db.SaveChangesAsync();

                var item = await _db.OrderItems
                    .FirstOrDefaultAsync(i => i.OrderId == order.Id && i.ServiceId == id);

                if (item != null)
                {
                    item.Quantity = (item.Quantity ?? 0) + 1;
                    _db.OrderItems.Update(item);
                }
                else
                {
                    var service = await _db.Services.FindAsync(id);
                    if (service != null)
                    {
                        _db.OrderItems.Add(new OrderItem
                        {
                            OrderId = order.Id,
                            ServiceId = id,
                            Quantity = 1,
                            UnitPrice = service.Price ?? 0m
                        });
                    }
                }

                await _db.SaveChangesAsync();
            }
            else
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItemSession>>("Cart") ?? new();
                var item = cart.FirstOrDefault(i => i.ServiceId == id);

                if (item != null)
                {
                    item.Quantity += 1;
                }
                else
                {
                    var service = await _db.Services.FindAsync(id);
                    if (service != null)
                    {
                        cart.Add(new CartItemSession
                        {
                            ServiceId = id,
                            Name = service.Name,
                            UnitPrice = service.Price ?? 0m,
                            Quantity = 1
                        });
                    }
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : RedirectToAction("MyCart");
        }

        [HttpPost]
        public async Task<IActionResult> Update(int orderItemId, int quantity)
        {
            var item = await _db.OrderItems.FindAsync(orderItemId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    _db.OrderItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                    _db.OrderItems.Update(item);
                }
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("MyCart");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int orderItemId)
        {
            var item = await _db.OrderItems.FindAsync(orderItemId);
            if (item != null)
            {
                _db.OrderItems.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("MyCart");
        }

        // Shto këtë për të kontrolluar pagesën
        public IActionResult ClickToPay()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ClickToPay", "Cart") });
            }

            // Këtu vazhdo me checkout ose pagesën
            return View("Checkout");
        }
        public async Task<IActionResult> Paid()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var paid = await _db.Orders
                .Where(o => o.PaymentStatus == "Completed" && o.ApplicationUserId == userId)
                .Include(o => o.ApplicationUser)
                .ToListAsync();

            return View(paid);
        }
    }

    // ViewModels për Cart
    public class CartIndexViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class CartItemViewModel
    {
        public int OrderItemId { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
