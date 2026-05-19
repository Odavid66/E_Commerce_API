using E_Commerce_API.Entities;

namespace E_commerce_API.DTOs
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
    }
}
