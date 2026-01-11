// ProjektSezon2/Services/ICookieConsentService.cs
using System.Threading.Tasks;
using ProjektSezon2.Models;

namespace ProjektSezon2.Services
{
    public interface ICookieConsentService
    {
       // Ruan ose përditëson preferencat e përdoruesit për cookies dhe kthen entitetin e ruajtur.
        /// <param name="consent">The CookieConsent entity with user preferences.</param>
       
        Task<CookieConsent> SaveConsentAsync(CookieConsent consent);

       // Merr regjistrimin më të fundit të pëlqimit për përdoruesin me ID specifike, ose null nëse nuk ekziston.
       /// <param name="userId">The user's ID.</param>
       Task<CookieConsent> GetConsentByUserAsync(string userId);
    }
}
