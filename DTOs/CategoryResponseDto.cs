namespace E_commerce_API.DTOs
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Published { get; set; } = DateTime.UtcNow;
        // date category was created
        public int ProductCount { get; set; }
        // number of products in the category
        public List<string> ProductName { get; set; } = [];
        //list of product in the category
    }
}
