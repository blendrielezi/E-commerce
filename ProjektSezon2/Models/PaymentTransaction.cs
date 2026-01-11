// ProjektSezon2/Models/PaymentTransaction.cs
namespace ProjektSezon2.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        public string? Provider { get; set; }  // PayPal, Stripe, etc.
        public string? TransactionId { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public decimal? Amount { get; set; }
    }
}
