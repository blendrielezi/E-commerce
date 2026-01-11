// ClientsController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Models;
using ProjektSezon2.Services;

namespace ProjektSezon2.Controllers
{
   
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICookieConsentService _cookieConsentService;

        public ClientsController(ApplicationDbContext db, ICookieConsentService cookieConsentService)
        {
            _db = db;
            _cookieConsentService = cookieConsentService;
        }

        // GET: /Clients/liste sherbmesh per klinetin ne varesi cookie
       
        public async Task<IActionResult> Index(int? categoryId, string? searchTerm)
        {
            var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            var servicesQuery = _db.Services.AsQueryable();

            if (categoryId.HasValue)
            {
                servicesQuery = servicesQuery.Where(s => s.CategoryId == categoryId.Value);
                ViewData["SelectedCategoryId"] = categoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                servicesQuery = servicesQuery.Where(s => s.Name.Contains(term) || s.Description.Contains(term));
                ViewData["SearchTerm"] = searchTerm;
            }

            var userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                : null;
            var consent = userId != null
                ? await _cookieConsentService.GetConsentByUserAsync(userId)
                : null;

            IQueryable<Service> finalQuery = consent?.Analytics == true
                ? servicesQuery.OrderBy(s => s.CategoryId).Take(10)
                : servicesQuery.OrderBy(s => Guid.NewGuid()).Take(10);

            var services = await finalQuery.ToListAsync();

            var model = new ClientServiceListViewModel
            {
                Categories = categories,
                Services = services
            };

            return View(model);
        }

        // GET: /Clients/Details/5
      
        public async Task<IActionResult> Details(int id)
        {
            var service = await _db.Services.FindAsync(id);
            if (service == null) return NotFound();

            var model = new ClientServiceDetailsViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price.GetValueOrDefault(),
                ImagePath = service.ImagePath
            };
            return View(model);
        }
    }

    internal class ClientServiceListViewModel
    {
        public List<Category> Categories { get; set; } = new();
        public List<Service> Services { get; set; } = new();
    }

    internal class ClientServiceDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
    }
}