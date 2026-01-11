using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Models;

namespace ProjektSezon2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("ByCategory/{categoryId}")]
        public async Task<IActionResult> GetServicesByCategory(int categoryId)
        {
            
            var services = await _context.Services
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();

            if (services == null || !services.Any())
            {
                return NotFound("Nuk ka shërbime për këtë kategori.");
            }

            return Ok(services);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromForm] string name, [FromForm] string? description, [FromForm] decimal price, [FromForm] IFormFile? image)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            service.Name = name;
            service.Description = description;
            service.Price = price;

            if (image != null && image.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                service.ImagePath = $"/uploads/{image.FileName}";
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> CreateService([FromForm] string name, [FromForm] string? description, [FromForm] decimal price, [FromForm] int categoryId, [FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Imazhi është i detyrueshëm.");

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var service = new Service
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                ImagePath = $"/uploads/{image.FileName}"
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return Ok(service);
        }


    }
}
