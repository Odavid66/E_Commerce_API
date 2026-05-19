using E_commerce_API.DTOs;
using E_Commerce_API.Data;
using E_Commerce_API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_API.Services
{
    public class CartService
    {
        private readonly DataContext _context;
        public CartService(DataContext context)
        {
            _context = context;
        }

        public async Task<Cart?> CreateCartAsync(UserCartRequestDto request)
        {
            var cart = await _context.Carts.Include(c => c.User).FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (cart is null)
            {
                var newCart = new Cart
                {
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(newCart);

                await _context.SaveChangesAsync();
                return newCart;
            }
            return cart;
        }

        public async Task<string?> AddToCartAsync(UserCartRequestDto request)
        {
            var cart = CreateCartAsync(request).Result;
            if (cart is null)
            {
                return null;
            }
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == request.ProductId);
            if (cartItem is null)
            {
                var newcartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Name = request.ProductName,
                    Quantity = 1
                };
                _context.CartItems.Add(newcartItem);

                cart.CartItems ??= new List<CartItem>();
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

        public async Task<string?> RemoveFromCartAsync(UserCartRequestDto request)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == request.UserId);
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

        public async Task<string?> DeleteCartAsync(UserCartRequestDto request)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == request.UserId);
            if (cart is null)
            {
                return null;
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return "success";
        }

        public async Task<List<CartResponseDto>> GetCartByUserIdAsync(UserCartRequestDto request)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == request.UserId);
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
