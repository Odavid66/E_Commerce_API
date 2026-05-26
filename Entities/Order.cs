using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_API.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem>? OrderItems { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User? User { get; set; }
        public Payment? Payment { get; set; }
    }
}
