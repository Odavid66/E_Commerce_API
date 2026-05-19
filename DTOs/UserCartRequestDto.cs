namespace E_commerce_API.DTOs
{
    public class UserCartRequestDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
    }
}
