namespace E_Commerce_API.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Cart? Cart { get; set; }
        public List<Order> Orders { get; set; } = [];
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
