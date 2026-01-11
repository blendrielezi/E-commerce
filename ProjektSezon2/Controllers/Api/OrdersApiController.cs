using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektSezon2.Models;
using ProjektSezon2.Data;
using Microsoft.EntityFrameworkCore;

namespace ProjektSezon2.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class OrdersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public OrdersApiController(ApplicationDbContext db)
        {
            _db = db;
        }
        //porosi,artikuj ,sherbime
        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Service)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
