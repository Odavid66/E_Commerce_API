using E_commerce_API.DTOs;
using E_Commerce_API.Data;
using E_Commerce_API.Entities;
using Microsoft.EntityFrameworkCore;


namespace E_commerce_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        public OrderService(DataContext context )
        {
            _context = context;
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            try
            {
                List<Order> orders = await _context.Orders.ToListAsync();
                if (orders is null)
                {
                    return [];
                }
                return orders.Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    OrderItems = o.OrderItems
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred while fetching orders: {ex.Message}");
                return [];
            }
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order is null)
                {
                    return null;
                }
                return new OrderResponseDto
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    OrderItems = order.OrderItems
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while fetching the order: {ex.Message}");
                return null;
            }
        }

        public async Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Orders).FirstOrDefaultAsync(u => u.Id == userId);
                if (user is null || user.Orders is null)
                {
                    return [];
                }
                return user.Orders.Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    OrderItems = o.OrderItems
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while fetching orders for user {userId}: {ex.Message}");
                return [];
            }
        }

        public async Task<int> TurnCartToOrderAsync(int userId)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Cart).ThenInclude(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(u => u.Id == userId);
                var cart = user?.Cart;
                var items = user?.Cart?.CartItems;
                decimal TotalPrice = 0;

                if (user is null || cart is null || items is null || !items.Any())
                {
                    return 0;
                }
                var products = items.Select(i => i.Product).ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var product = products[i];
                    if (item.Quantity > product.Stock)
                    {
                        return -1;
                    }
                    TotalPrice += (decimal)(item.Quantity * item.Product.Price);
                    product.Stock -= item.Quantity;

                }

                var order = new Order
                {
                    UserId = userId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = TotalPrice,
                    OrderItems = items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.Product.Price,
                        Product = i.Product
                    }).ToList()
                };
                user.Orders ??= [];
                user.Orders.Add(order);
                _context.Orders.Add(order);
                _context.OrderItems.AddRange(order.OrderItems);
                _context.CartItems.RemoveRange(items);
                _context.Carts.Remove(cart);

                await _context.SaveChangesAsync();

                return order.Id;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while turning cart to order: {ex.Message}");
                return 0;
            }

        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order is null)
                {
                    return false;
                }
                order.Status = newStatus;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while updating the order status: {ex.Message}");
                return false;
            }
        }
    }
}
