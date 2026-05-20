using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_API.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        [ForeignKey("CategoryId")]
        public int? CategoryId { get; set; }
        public Category? ProductCategory { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public List<CartItem> CartItems { get; set; } = [];
       
    }
}
