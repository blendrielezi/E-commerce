using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektSezon2.Models;
using ProjektSezon2.Services;

namespace ProjektSezon2.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsApiController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        public SessionsApiController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        //lista e perdoruesve te identifikuar
        [HttpGet]
        public async Task<IEnumerable<Session>> Get()
        {
            var userId = User.FindFirst("sub")?.Value;
            return await _sessionService.GetUserSessionsAsync(userId);
        }
    }
}
