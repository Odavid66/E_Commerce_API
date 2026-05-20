using E_Commerce_API.DTOs;
using E_Commerce_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// User registration details(email, password, firstName, lastName)
        /// <returns>Success message and user details</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid input data"
                });
            }

            var result = await _authService.Register(request);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetProfile), new { }, result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Login user and receive JWT token
        /// </summary>
        /// <param name="request">Login credentials (email, password)</param>
        /// <returns>JWT token and user details</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid input data"
                });
            }

            var result = await _authService.Login(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <returns>New access token and refresh token</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Refresh token is required"
                });
            }

            var result = await _authService.RefreshAccessToken(request.RefreshToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }

        /// <summary>
        /// Get logged-in user profile information
        /// </summary>
        /// <returns>User profile details</returns>
        [HttpGet("profile")]
        [Authorize]
        public ActionResult<UserDto> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = _authService.GetUserProfile(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Logout user by revoking all refresh tokens
        /// </summary>
        /// <returns>Logout confirmation</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<AuthResponse>> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var result = await _authService.Logout(userId);

            if (result)
            {
                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Logout successful"
                });
            }

            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Logout failed"
            });
        }
    }
}
