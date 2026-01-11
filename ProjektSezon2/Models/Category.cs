namespace ProjektSezon2.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImagePath { get; set; }

        // Lidhja me kategorin prind
        public int? ParentCategoryId { get; set; }
        
        // Lidhja me sherbimet
        public ICollection<Service>? Services { get; set; }
    }
}
