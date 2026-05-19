using E_commerce_API.DTOs;
using E_commerce_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartByUserId([FromQuery] UserCartRequestDto request)
        {
            var cart = await _cartService.GetCartByUserIdAsync(request);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] UserCartRequestDto request)
        {
            var result = await _cartService.AddToCartAsync(request);
            if (result is null)
            {
                return BadRequest("Failed to add item to cart");
            }
            var item = request.ProductName;
            return Ok($"{item} added to cart");
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] UserCartRequestDto request)
        {
            var result = await _cartService.RemoveFromCartAsync(request);
            if (result is null)
            {
                return BadRequest("Failed to remove item from cart");
            }
            var item = request.ProductName;
            return Ok($"{item} removed from cart");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCart([FromBody] UserCartRequestDto request)
        {
            var result = await _cartService.DeleteCartAsync(request);
            if (result is null)
            {
                return BadRequest("Failed to delete cart");
            }
            return Ok("Empty cart");
        }


    }
}
