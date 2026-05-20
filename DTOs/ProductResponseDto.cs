using E_Commerce_API.Entities;

namespace E_commerce_API.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string ImageUrl { get; set; } = string.Empty;
        }
}
