using E_commerce_API.DTOs;
using E_commerce_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        // ↑
        // controller talks to the interface
        // not directly to DataContext anymore
        // no database code in the controller

        public CategoryController(ICategoryService categoryService)
        // ↑
        // ASP.NET injects ICategoryService automatically
        // because we registered it in Program.cs
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryResponseDto>>> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result is null)
                return NotFound($"Category with id {id} does not exist");
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CategoryDto dto)
        {
            var nameExists = await _categoryService.CategoryNameExistsAsync(dto.Name);
            if (nameExists)
                return BadRequest("Category already exists");

            var result = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryResponseDto>> UpdateCategory(int id, CategoryDto dto)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, dto);
            if (result is null)
                return NotFound($"Category with id {id} does not exist");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
                return NotFound($"Category with id {id} does not exist");
            return NoContent();
        }
    }
}