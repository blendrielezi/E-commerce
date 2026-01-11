// ProjektSezon2/Models/FilterSetting.cs
namespace ProjektSezon2.Models
{
    public class FilterSetting
    {
        public int Id { get; set; }
        public string? Name { get; set; }     // e.g., "peridhadatave", "Status"
        public string? Value { get; set; }    // JSON or comma-separated
        public DateTime? CreatedAt { get; set; }
    }
}