using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_API.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public decimal Amount { get; set; } 
        public string Status { get; set; } = string.Empty;
        public string? PaystackReference { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
