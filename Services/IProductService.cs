using E_commerce_API.DTOs;

namespace E_commerce_API.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<List<ProductResponseDto>> GetByCategoryAsync(int categoryId);
        Task<List<ProductResponseDto>> SearchByNameAsync(string name);
        Task<(ProductResponseDto? product, string? error)> CreateProductAsync(ProductDto dto);
        // ↑
        // returns a tuple — either the created product or an error message
        // tuple lets us return two things at once without a separate class
        Task<(ProductResponseDto? product, string? error)> UpdateProductAsync(int id, ProductDto dto);
        Task<bool> DeleteProductAsync(int id);
    }
}