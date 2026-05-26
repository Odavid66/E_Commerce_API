using E_commerce_API.DTOs;
using E_commerce_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce_API.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out userId);
        }
        private readonly IOrderService _orderservice;

        public OrderController( IOrderService orderService)
        {
            _orderservice = orderService;
        }

        [Authorize]
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderDto?>> Checkout()
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }
            var orderId = await _orderservice.TurnCartToOrderAsync(userId);
            if (orderId is 0)
            {
                return BadRequest("Checkout failed. Cart is empty.");
            }
            if(orderId is -1)
            {
                return BadRequest("Checkout failed. Cart item out of stock.");
            }
            OrderDto? order = await _orderservice.GetOrderByIdAsync(orderId);
            if(order is null)
            {
                return NotFound("Order not found after checkout.");
            }
            return Ok(order );
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetUserOrders()
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }
            List<OrderDto> orders = await _orderservice.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderByOrderId(int id)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }
            OrderDto? order = await _orderservice.GetOrderByIdAsync(id);
            if (order is null)
            {
                return NotFound("Order not found.");
            }
            return Ok(order);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            List<OrderDto> orders = await _orderservice.GetAllOrdersAsync();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            bool success = await _orderservice.UpdateOrderStatusAsync(id, newStatus);
            if (!success)
            {
                return NotFound("Order not found or status update failed.");
            }
            return NoContent();

        }   
    }
}
