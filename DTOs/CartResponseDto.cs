namespace E_commerce_API.DTOs
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public int productId { get; set; }
        public string productImg { get; set; } = string.Empty;
        public string productdescription { get; set; } = string.Empty;
        public decimal productprice { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
