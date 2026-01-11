namespace ProjektSezon2.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? ImagePath { get; set; }

        // Lidhja me kategorinë
        public int? CategoryId { get; set; } // Foreign Key per lidhjen me Category
        public Category? Category { get; set; } // Objekt i kategorisë
    }
}
