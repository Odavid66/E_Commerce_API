namespace E_commerce_API.DTOs
{
    public class CategoryDto
    // shape of data the client want to see when retrieving or creating category
    {
        public required string Name { get; set; } = string.Empty;
        // category name — must be provided
        public string Description { get; set; } = string.Empty;
    }
}
