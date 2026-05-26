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

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                List<Order> orders = await _context.Orders.Include(o => o.OrderItems).ToListAsync();
                if(orders is null)
                {
                    return [];
                }
                return orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    OrderItemDtos = o.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        ProductId = oi.ProductId
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred while fetching orders: {ex.Message}");
                return [];
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
                if (order is null || order.OrderItems is null)
                {
                    return null;
                }
                var orderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ProductId = oi.ProductId
                }).ToList();
                return new OrderDto
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    OrderItemDtos = orderItems
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while fetching the order: {ex.Message}");
                return null;
            }
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Orders).ThenInclude(o => o.OrderItems).FirstOrDefaultAsync(u => u.Id == userId);
                if (user is null || user.Orders is null)
                {
                    return [];
                }
                return user.Orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    OrderItemDtos = o.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        ProductId = oi.ProductId
                    }).ToList()
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
                var orderItems = items.Select(i => new OrderItemDto 
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product.Price,
                }).ToList();

                var order = new OrderDto
                {
                    UserId = userId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = TotalPrice,
                    OrderItemDtos = orderItems 
                };
               
                var Order = new Order
                {
                    UserId = order.UserId,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    TotalAmount = order.TotalAmount,
                    OrderItems = order.OrderItemDtos.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        UnitPrice = (decimal)i.UnitPrice,
                        Quantity = i.Quantity
                    }).ToList()

                };
                user.Orders ??= [];
                user.Orders.Add(Order);

                
                _context.OrderItems.AddRange(Order.OrderItems);
                _context.CartItems.RemoveRange(items);
                _context.Carts.Remove(cart);

                await _context.SaveChangesAsync();

                return Order.Id;
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
