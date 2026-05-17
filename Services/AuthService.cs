using E_Commerce_API.Data;
using E_Commerce_API.DTOs;
using E_Commerce_API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace E_Commerce_API.Services
{
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email and password are required"
                    };
                }

                // Check if email already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email already registered"
                    };
                }

                // Hash password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new user
                var newUser = new User
                {
                    Email = request.Email,
                    Password = hashedPassword,
                    Firstname = request.FirstName,
                    Lastname = request.LastName,
                    Role = "Customer",
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Create response
                var userDto = new UserDto
                {
                    Id = newUser.Id,
                    Email = newUser.Email,
                    FirstName = newUser.Firstname,
                    LastName = newUser.Lastname,
                    Role = newUser.Role
                };

                return new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful",
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email and password are required"
                    };
                }

                // Find user by email
                var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Verify password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Generate Access Token and Refresh Token
                var accessToken = GenerateAccessToken(user);
                var refreshToken = await GenerateAndSaveRefreshToken(user);

                // Create response
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.Firstname,
                    LastName = user.Lastname,
                    Role = user.Role
                };

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        private string GenerateAccessToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var accessTokenExpiry = int.TryParse(_configuration["Jwt:AccessTokenExpiryMinutes"], out var minutes) ? minutes : 60;

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "ECommerceAPI",
                audience: _configuration["Jwt:Audience"] ?? "ECommerceUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(accessTokenExpiry),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("type", "refresh")
            };

            var refreshTokenExpiry = int.TryParse(_configuration["Jwt:RefreshTokenExpiryDays"], out var days) ? days : 7;
            var expiryDate = DateTime.UtcNow.AddDays(refreshTokenExpiry);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "ECommerceAPI",
                audience: _configuration["Jwt:Audience"] ?? "ECommerceUsers",
                claims: claims,
                expires: expiryDate,
                signingCredentials: creds);

            var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Save refresh token to database
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenString,
                ExpiryDate = expiryDate,
                CreatedAt = DateTime.UtcNow,
                IsUsed = false,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshTokenString;
        }

        public UserDto? GetUserProfile(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Role = user.Role
            };
        }

        public async Task<AuthResponse> RefreshAccessToken(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Refresh token is required"
                    };
                }

                // Find the stored refresh token
                var storedToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(t => t.Token == refreshToken);

                // Check if token exists and is valid
                if (storedToken == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid refresh token"
                    };
                }

                if (storedToken.IsRevoked)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Refresh token has been revoked"
                    };
                }

                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Refresh token has expired"
                    };
                }

                // Double-spend attack detection: Token already used
                if (storedToken.IsUsed)
                {
                    // Security alert: Someone is trying to reuse a refresh token!
                    // Revoke all tokens for this user
                    await RevokeAllUserTokens(storedToken.UserId);

                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Token reuse detected! All tokens have been revoked for security. Please login again."
                    };
                }

                // Mark old token as used
                storedToken.IsUsed = true;
                storedToken.RevokedAt = DateTime.UtcNow;

                // Get user
                var user = storedToken.User;
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Generate new tokens
                var newAccessToken = GenerateAccessToken(user);
                var newRefreshToken = await GenerateAndSaveRefreshToken(user);

                // Save changes
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.Firstname,
                    LastName = user.Lastname,
                    Role = user.Role
                };

                return new AuthResponse
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Token refresh failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> RevokeAllUserTokens(int userId)
        {
            try
            {
                var userTokens = await _context.RefreshTokens
                    .Where(t => t.UserId == userId && !t.IsRevoked)
                    .ToListAsync();

                foreach (var token in userTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Logout(int userId)
        {
            return await RevokeAllUserTokens(userId);
        }
    }
}
