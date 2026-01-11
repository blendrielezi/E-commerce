using System;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektSezon2.Models;
using ProjektSezon2.Services;

namespace ProjektSezon2.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CookieConsentController : ControllerBase
    {
        private readonly ICookieConsentService _consentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CookieConsentController(
            ICookieConsentService consentService,
            UserManager<ApplicationUser> userManager)
        {
            _consentService = consentService;
            _userManager = userManager;
        }

        /// ruan ose perditeson cookien e perdoruesit dhe vendos nje cookie te mos shfaqe banerin
        
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] CookieConsentDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var consent = new CookieConsent
            {
                ApplicationUserId = user.Id,
                Functional = dto.Functional,
                Analytics = dto.Analytics,
                Marketing = dto.Marketing,
                ConsentedAt = DateTime.UtcNow
            };

            var result = await _consentService.SaveConsentAsync(consent);

            // vendos nserver nje cookie me prmbajtje JSON te koduar ne URL,i vlefshem 1 vit
            var json = Uri.EscapeDataString(
                JsonSerializer.Serialize(new
                {
                    functional = result.Functional,
                    analytics = result.Analytics,
                    marketing = result.Marketing
                })
            );

            Response.Cookies.Append(
                "UserCookieConsent",
                json,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(365),
                    HttpOnly = false,
                    Path = "/"
                }
            );

            return Ok(new
            {
                result.Functional,
                result.Analytics,
                result.Marketing,
                result.ConsentedAt
            });
        }
    }

    /// transfero cookie nga klienti per serverin
    
    public class CookieConsentDto
    {
        public bool Functional { get; set; }
        public bool Analytics { get; set; }
        public bool Marketing { get; set; }
    }
}
