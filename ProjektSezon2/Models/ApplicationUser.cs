// ProjektSezon2/Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace ProjektSezon2.Models
{
    public class ApplicationUser : IdentityUser
    {
        // profili zgjeruar (fusha nullable )
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? City { get; set; }

        // Navigation (nullable collections)
        public virtual ICollection<Session>? Sessions { get; set; }
        public virtual ICollection<CookieConsent>? CookieConsents { get; set; }
    }
}