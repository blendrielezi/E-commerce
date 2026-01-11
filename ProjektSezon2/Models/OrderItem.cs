// ProjektSezon2/Models/OrderItem.cs
namespace ProjektSezon2.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
