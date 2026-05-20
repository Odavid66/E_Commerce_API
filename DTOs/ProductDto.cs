namespace E_commerce_API.DTOs
{
    public class ProductDto
    {
        public required string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        // nullable — product can exist without a category
    }
}
