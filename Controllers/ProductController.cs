using E_commerce_API.DTOs;
using E_commerce_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponseDto>>> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result is null)
                return NotFound($"Product with id {id} does not exist");
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<ProductResponseDto>>> GetByCategory(int categoryId)
        {
            var result = await _productService.GetByCategoryAsync(categoryId);
            if (!result.Any())
                return NotFound($"No products found in category {categoryId}");
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductResponseDto>>> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Please provide a search term");

            var result = await _productService.SearchByNameAsync(name);
            if (!result.Any())
                return NotFound($"No products found matching '{name}'");
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct(ProductDto dto)
        {
            var (product, error) = await _productService.CreateProductAsync(dto);
            // ↑
            // destructure the tuple into two variables
            // product = the created product or null
            // error   = the error message or null

            if (error is not null)
                return BadRequest(error);
            // ↑
            // if there is an error message return 400

            return CreatedAtAction(nameof(GetProduct), new { id = product!.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, ProductDto dto)
        {
            var (product, error) = await _productService.UpdateProductAsync(id, dto);

            if (product is null && error is null)
                return NotFound($"Product with id {id} does not exist");
            // ↑
            // null product AND null error = not found

            if (error is not null)
                return BadRequest(error);
            // ↑
            // null product but has error = bad request

            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound($"Product with id {id} does not exist");
            return NoContent();
        }
    }
}
