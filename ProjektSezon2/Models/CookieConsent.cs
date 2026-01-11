// ProjektSezon2/Models/CookieConsent.cs
namespace ProjektSezon2.Models
{
    public class CookieConsent
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public bool? Functional { get; set; }
        public bool? Analytics { get; set; }
        public bool? Marketing { get; set; }
        public DateTime? ConsentedAt { get; set; }
    }
}
