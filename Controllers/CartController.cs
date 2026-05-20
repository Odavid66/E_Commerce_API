using E_commerce_API.DTOs;
using E_commerce_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out userId);
        }
        public CartController(ICartService cartService)  
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartByUserId()
        {
            if (!TryGetUserId(out int UserId))
            {
                return Unauthorized();
            }   
            var cart = await _cartService.GetCartByUserIdAsync(UserId);
            return Ok(cart);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] UserCartRequestDto request)
        {
            if (!TryGetUserId(out int UserId))
            {
                return Unauthorized();
            }
            var result = await _cartService.AddToCartAsync(request, UserId);
            if (result is null)
            {
                return BadRequest("Failed to add item to cart");
            }
            var item = request.ProductName;
            return Ok($"{item} added to cart");
        }

        [HttpDelete("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(UserCartRequestDto request)
        {
            if (!TryGetUserId(out int UserId))
            {
                return Unauthorized();
            }
            var result = await _cartService.RemoveFromCartAsync(request, UserId);
            if (result is null)
            {
                return BadRequest("Failed to remove item from cart");
            }
            var item = request.ProductName;
            return Ok($"{item} removed from cart");
        }

        [HttpDelete("DeleteCart")]
        public async Task<IActionResult> DeleteCart(UserCartRequestDto request)
        {
            if (!TryGetUserId(out int UserId))
            {
                return Unauthorized();
            }
            var result = await _cartService.DeleteCartAsync(request, UserId);
            if (result is null)
            {
                return BadRequest("Failed to delete cart");
            }
            return Ok("Empty cart");
        }


    }
}
