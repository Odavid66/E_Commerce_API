using E_Commerce_API.Entities;

namespace E_commerce_API.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto>? OrderItemDtos { get; set; }
        public int UserId { get; set; }
    }
}
