using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Models;

namespace ProjektSezon2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Konstruktor që merr ApplicationDbContext
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        //metode per afishimin e kategorise
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories
                .Include(c => c.Services)
                .ToListAsync();
        }


        //metode qe shton kategori te re
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromForm] string name, [FromForm] string? description, [FromForm] IFormFile? image)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Emri i kategorisë është i detyrueshëm.");

            var category = new Category
            {
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            if (image != null && image.Length > 0)
            {
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                var filePath = Path.Combine(uploadDirectory, image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                category.ImagePath = $"/uploads/{image.FileName}";
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Redirekto përdoruesin pas krijimit të kategorisë
            return RedirectToAction("Index", "Home");
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id)
                return BadRequest();

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(c => c.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }



    }
}
