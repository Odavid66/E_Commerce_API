using E_commerce_API.DTOs;

namespace E_commerce_API.Services
{
    public interface ICategoryService
    // ↑
    // interface defines WHAT the service can do
    // not HOW it does it
    // controller talks to this interface — not the concrete class
    // makes it easy to swap implementations later
    {
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryDto dto);
        Task<CategoryResponseDto?> UpdateCategoryAsync(int id, CategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
        // ↑
        // returns bool — true if deleted, false if not found

        Task<bool> CategoryNameExistsAsync(string name);
    }
}