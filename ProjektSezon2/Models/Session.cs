// ProjektSezon2/Models/Session.cs
namespace ProjektSezon2.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? IpAddress { get; set; }
    }
}