using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_API.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        public string Name { get; set; } = string.Empty;

        [ForeignKey("CartId")]
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public required Product Product { get; set; }
    }
}
