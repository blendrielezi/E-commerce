// 4. Controllers/SessionsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektSezon2.Services;
using ProjektSezon2.Models;
using Microsoft.AspNetCore.Identity;

namespace ProjektSezon2.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly UserManager<ApplicationUser> _userManager;
        public SessionsController(ISessionService sessionService, UserManager<ApplicationUser> um)
        {
            _sessionService = sessionService;
            _userManager = um;
        }
        //merr sesionet e perdoruesit e i kthen per ti shfaq ne view
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var sessions = await _sessionService.GetUserSessionsAsync(user.Id);
            return View(sessions);
        }
    }
}