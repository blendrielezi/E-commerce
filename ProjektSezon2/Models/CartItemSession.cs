namespace ProjektSezon2.Models
{
    public class CartItemSession
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
