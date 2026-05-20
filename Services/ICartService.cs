using E_commerce_API.DTOs;


namespace E_commerce_API.Services
{
    public interface ICartService
    {
        public Task<string?> AddToCartAsync(UserCartRequestDto request, int UserId);
        public Task<string?> RemoveFromCartAsync(UserCartRequestDto request, int UserId);
        public Task<string?> DeleteCartAsync(UserCartRequestDto request, int UserId);
        public Task<List<CartResponseDto>> GetCartByUserIdAsync(int UserId);

    }
}
