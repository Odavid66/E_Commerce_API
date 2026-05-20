using E_Commerce_API.DTOs;

namespace E_Commerce_API.Services
{
    /// <summary>
    /// Interface for authentication service
    /// Defines contract for user registration, login, token management
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponse> Register(RegisterRequest request);
        Task<AuthResponse> Login(LoginRequest request);
        Task<AuthResponse> RefreshAccessToken(string refreshToken);
        Task<bool> RevokeAllUserTokens(int userId);
        Task<bool> Logout(int userId);
        UserDto? GetUserProfile(int userId);
    }
}