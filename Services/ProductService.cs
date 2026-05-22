using E_commerce_API.DTOs;
using E_commerce_API.Services.Interfaces;
using E_Commerce_API.Data;
using E_Commerce_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_API.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        // ── helper to map Product → ProductResponseDto ─────────────
        private static ProductResponseDto MapToDto(Product p)
        // ↑
        // private static helper method
        // avoids repeating the same mapping code in every method
        // static means it does not need an instance — no _context needed
        {
            return new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.ProductCategory?.Name ?? "No Category",
                Created = p.Created
            };
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .ToListAsync();

            return products.Select(MapToDto).ToList();
            // ↑
            // MapToDto is the helper method above
            // cleaner than writing the mapping inline every time
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return null;

            return MapToDto(product);
        }

        public async Task<List<ProductResponseDto>> GetByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductResponseDto>> SearchByNameAsync(string name)
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            return products.Select(MapToDto).ToList();
        }

        public async Task<(ProductResponseDto? product, string? error)> CreateProductAsync(ProductDto dto)
        {
            // check name not already taken
            var nameExists = await _context.Products
                .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower());

            if (nameExists)
                return (null, "Product with this name already exists");
            // ↑
            // return tuple with null product and error message
            // controller reads this and returns BadRequest

            // check category if provided
            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.CategoryId.Value);

                if (!categoryExists)
                    return (null, $"Category with id {dto.CategoryId} does not exist");
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (product.CategoryId.HasValue)
            {
                await _context.Entry(product)
                    .Reference(p => p.ProductCategory)
                    .LoadAsync();
            }

            return (MapToDto(product), null);
            // ↑
            // return tuple with product and null error
            // controller reads this and returns 201 Created
        }

        public async Task<(ProductResponseDto? product, string? error)> UpdateProductAsync(int id, ProductDto dto)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
                return (null, null);
            // ↑
            // null product AND null error = not found
            // controller returns 404

            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.CategoryId.Value);

                if (!categoryExists)
                    return (null, $"Category with id {dto.CategoryId} does not exist");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            if (product.CategoryId.HasValue)
            {
                await _context.Entry(product)
                    .Reference(p => p.ProductCategory)
                    .LoadAsync();
            }

            return (MapToDto(product), null);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}