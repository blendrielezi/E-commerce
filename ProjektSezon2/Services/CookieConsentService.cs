// ProjektSezon2/Services/CookieConsentService.cs
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Models;

namespace ProjektSezon2.Services
{
    public class CookieConsentService : ICookieConsentService
    {
        private readonly ApplicationDbContext _db;

        public CookieConsentService(ApplicationDbContext db)
        {
            _db = db;
        }
        //ruan ose përditson plqimin e cookies per nje perdorues tcaktuar
        public async Task<CookieConsent> SaveConsentAsync(CookieConsent consent)
        {
            var existing = await _db.CookieConsents
                .FirstOrDefaultAsync(c => c.ApplicationUserId == consent.ApplicationUserId);

            if (existing != null)
            {
                existing.Functional = consent.Functional;
                existing.Analytics = consent.Analytics;
                existing.Marketing = consent.Marketing;
                existing.ConsentedAt = consent.ConsentedAt;
                _db.CookieConsents.Update(existing);
                await _db.SaveChangesAsync();
                return existing;
            }

            _db.CookieConsents.Add(consent);
            await _db.SaveChangesAsync();
            return consent;
        }
        //merr pe;qimin e cookies per nje perdorues nga databaza
        public async Task<CookieConsent> GetConsentByUserAsync(string userId)
        {
            return await _db.CookieConsents
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        }
    }
}
