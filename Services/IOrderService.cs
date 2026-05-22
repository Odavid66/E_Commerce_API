using E_commerce_API.DTOs;

namespace E_commerce_API.Services
{
    public interface IOrderService
    {
        public Task<int> TurnCartToOrderAsync( int userId);
        public Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId);
        public Task<OrderResponseDto?> GetOrderByIdAsync(int orderId);
        public Task<List<OrderResponseDto>> GetAllOrdersAsync();
        public Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
