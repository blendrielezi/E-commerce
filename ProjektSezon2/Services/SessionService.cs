// ProjektSezon2/Services/SessionService.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Models;
using ProjektSezon2.Extensions; // for GetIpAddress
using Microsoft.AspNetCore.Http;

namespace ProjektSezon2.Services
{
    public class SessionService : ISessionService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        //Krijon sesion te ri per perdoruesin aktual duke regjistruar kohn kur fillon sesioni dhe IP adresën e perdoruesit
        //Nse përdoruesi nuk gjendet, metoda nuk bn asgj
        //Nëse përdoruesi nuk gjendet, metoda nuk bën asgjë
        public async Task CreateSessionAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) return;

            var session = new Session
            {
                ApplicationUserId = user.Id,
                StartedAt = DateTime.UtcNow,
                IpAddress = principal.GetIpAddress(_httpContextAccessor)
            };

            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();
        }
       // Gjen sesionin e fundit aktiv(pa EndedAt) pr prdoruesin dhe regjistron kohn kur prfundon sesioni.
//Nse nuk ka sesion aktiv, sben asgje
        public async Task EndSessionAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) return;

            var session = await _db.Sessions
                .Where(s => s.ApplicationUserId == user.Id && s.EndedAt == null)
                .OrderByDescending(s => s.StartedAt)
                .FirstOrDefaultAsync();

            if (session != null)
            {
                session.EndedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
        //Kthen listën e sesioneve per pedoruesin me ID e dhen,renditura nga m i fundit t m i vjetri.


        public async Task<IEnumerable<Session>> GetUserSessionsAsync(string userId)
        {
            return await _db.Sessions
                .Where(s => s.ApplicationUserId == userId)
                .OrderByDescending(s => s.StartedAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
