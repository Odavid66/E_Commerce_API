using E_commerce_API.DTOs;

namespace E_commerce_API.Services
{
    public interface IOrderService
    {
        public Task<int> TurnCartToOrderAsync( int userId);
        public Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId);
        public Task<OrderDto?> GetOrderByIdAsync(int orderId);
        public Task<List<OrderDto>> GetAllOrdersAsync();
        public Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
