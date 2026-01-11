using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ProjektSezon2.Models;

namespace ProjektSezon2.Filters
{
    public class AdminFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AdminFilter(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // ekzekoutohet vtm ne contrrolelr
            if (context.Controller is Controller controller &&
                context.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(context.HttpContext.User);
                if (user != null)
                {
                    // check role
                    bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                    // merr special ID from config (e.g. appsettings.json)
                    var specialAdminId = _configuration["SpecialAdminId"];

                    //  vendos nese shfaqet karta 4
                    controller.ViewData["ShowFourthCard"] = (isAdmin && user.Id == specialAdminId);
                }
            }

            // len actionin te ekzekutoeht
            await next();
        }
    }
}