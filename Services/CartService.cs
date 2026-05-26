using E_commerce_API.DTOs;
using E_Commerce_API.Data;
using E_Commerce_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_API.Services
{
    public class CartService: ICartService
    {
        private readonly DataContext _context;
        public CartService(DataContext context)
        {
            _context = context;
        }

        private async Task<Cart> CreateCartAsync(int UserId)
        {
            var cart = await _context.Carts.Include(c => c.User).FirstOrDefaultAsync(u => u.UserId == UserId);
            if (cart is null)
            {
                var newCart = new Cart
                {
                    UserId = UserId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(newCart);

                try
                {
                    await _context.SaveChangesAsync();
                    return newCart;
                }
                catch (DbUpdateException)
                {
                    _context.Entry(newCart).State = EntityState.Detached;
                    cart = await _context.Carts
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(u => u.UserId == UserId);
                    if (cart is not null)
                    {
                        return cart;
                    }
                    throw;
                }
            }
            return cart;
        }

        public async Task<string?> AddToCartAsync(UserCartRequestDto request, int UserId)
        {
            var cart = await CreateCartAsync(UserId);
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == request.ProductId);
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (product is not null)
            {
                if (cartItem is null)
                {
                    var newcartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        Name = product.Name,
                        Quantity = 1,
                        Product = product
                    };
                    _context.CartItems.Add(newcartItem);

                    
                    cart.CartItems.Add(newcartItem);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    cartItem.Quantity += 1;
                    _context.CartItems.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                return "success";
            }
            return null;

        }

        public async Task<string?> RemoveFromCartAsync(UserCartRequestDto request, int UserId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == UserId);
            if (cart is null)
            {
                return null;
            }
            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (cartItem is null)
            {
                return null;
            }
            else if (cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return "success";
        }

        public async Task<string?> DeleteCartAsync(UserCartRequestDto request, int UserId)
        { 
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == UserId);
            if (cart is null)
            {
                return null;
            }

            _context.Carts.Remove(cart);
            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
            return "success";
        }

        public async Task<List<CartResponseDto>> GetCartByUserIdAsync(int UserId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == UserId);
            if (cart is null)
            {
                return [];
            }
            if (cart.CartItems?.Count > 0)
            {
                var response = cart.CartItems.Select(t => new CartResponseDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Quantity = t.Quantity
                }).ToList();

                return response;
            }
            return [];
        }
    }
}
