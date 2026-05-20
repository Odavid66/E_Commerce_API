using E_commerce_API.DTOs;
using E_commerce_API.Services.Interfaces;
using E_Commerce_API.Data;
using E_Commerce_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_API.Services
{
    public class CategoryService : ICategoryService
    // ↑
    // implements the interface — must have all methods defined there
    {
        private readonly DataContext _context;

        public CategoryService(DataContext context)
        // ↑
        // DataContext injected here instead of in controller
        // controller no longer needs direct database access
        {
            _context = context;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count,
                ProductName = c.Products.Select(p => p.Name).ToList()
            }).ToList();
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        // ↑
        // returns null if not found — controller handles the 404
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return null;

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.Products.Count,
                ProductName = category.Products.Select(p => p.Name).ToList()
            };
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = 0,
                ProductName = []
            };
        }

        public async Task<CategoryResponseDto?> UpdateCategoryAsync(int id, CategoryDto dto)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return null;

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.Products.Count,
                ProductName = category.Products.Select(p => p.Name).ToList()
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CategoryNameExistsAsync(string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }
}