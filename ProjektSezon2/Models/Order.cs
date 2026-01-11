// ProjektSezon2/Models/Order.cs
namespace ProjektSezon2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderItem>? Items { get; set; }
        public ICollection<PaymentTransaction>? PaymentTransactions { get; set; }

        public decimal? TotalAmount { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
