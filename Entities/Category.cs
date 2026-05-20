namespace E_Commerce_API.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = [];
        // needed for .Include(c => c.Products) in CategoryController
    }
}
