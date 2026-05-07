using DeskFlow.Core.Domain.IdentityEntity;
using DeskFlow.Core.DTOs.AccountDTOs;
using DeskFlow.Core.DTOs.shared;
using DeskFlow.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace DeskFlow.WebAPI.Controllers
{
    /// <summary>
    /// Authentication controller for handling user login, token refresh, and password reset operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwt;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the AuthController class with the specified user manager, JWT service, and
        /// logger.
        /// </summary>
        /// <param name="userManager">The user manager used to manage and validate application users.</param>
        /// <param name="jwtService">The service used to generate and validate JSON Web Tokens for authentication.</param>
        /// <param name="logger">The logger used to record authentication-related events and errors.</param>
        public AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _jwt = jwtService;
            _logger = logger;
        }


        /// <summary>
        /// Handles user login
        /// </summary>
        /// <param name="loginDTO">The login data transfer object containing the user's email and password.</param>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            var isPasswordValid = user != null && await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (user == null || !isPasswordValid)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", loginDTO.Email);
                return Unauthorized(new ApiErrorResponse
                {
                    Message = "Invalid email or password",
                    Errors = new List<string> { "The provided email or password is incorrect." },
                    TraceId = HttpContext.TraceIdentifier
                });

            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Inactive user login attempt for email: {Email}", loginDTO.Email);
                return Unauthorized(new ApiErrorResponse
                {
                    Message = "User account is inactive",
                    Errors = new List<string> { "The user account is inactive." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            
            var roles = await _userManager.GetRolesAsync(user);
            AuthResponseDTO authResponse = await _jwt.GenerateToken(user,roles);
            _logger.LogInformation("User {Email} logged in successfully", loginDTO.Email);
            return Ok(authResponse);

        }



        /// <summary>
        /// refreshes the access token using a valid refresh token.
        /// </summary>
        /// <remarks>The client must provide both the expired access token and a valid refresh token. If the refresh token is invalid, expired
        /// , or does not match the user, the request will be rejected.</remarks>
        /// <param name="request">The refresh token request containing the expired access token and the associated refresh token.</param>
        /// <returns> Return the new access token and refresh token if successful</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> refreshToken(RefreshTokenRequestDTO request)
        {
                var principal = _jwt.GetPrincipalFromExpiredToken(request.Token);

                if(principal == null)
                {
                    ApiErrorResponse errorResponse = new ApiErrorResponse
                    {
                        Message = "Invalid token",
                        Errors = new List<string> { "The provided access token is invalid or malformed." },
                        TraceId = HttpContext.TraceIdentifier
                    };
                    return BadRequest(errorResponse);
                }

                string? userEmail = principal.FindFirstValue(ClaimTypes.Email);
 
            ApplicationUser? user = await _userManager.FindByEmailAsync(userEmail!);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    ApiErrorResponse errorResponse = new ApiErrorResponse
                    {
                        Message = "Invalid token or refresh token",
                        Errors = new List<string> { "The provided refresh token is invalid, expired, or does not match the user." },
                        TraceId = HttpContext.TraceIdentifier
                    };
                    return BadRequest(errorResponse);
                }

                var roles = await _userManager.GetRolesAsync(user);
                var response = await _jwt.GenerateToken(user, roles);
                return Ok(response);
            

        }



        /// <summary>
        /// Resets the password for a user identified by the provided email address.
        /// </summary>
        /// <param name="passwordRequest">An object containing the user's email address, current password, and new password.</param>
        /// <returns>An HTTP 200 OK response if the password is successfully reset</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> resetPassword(ResetPasswordRequestDTO passwordRequest)
        {
            var UserEmail = User.FindFirstValue(ClaimTypes.Email);
            ApplicationUser? user = await _userManager.FindByEmailAsync(UserEmail!);

            if (user == null)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "User not found",
                    Errors = new List<string> { $"No user found with the email '{UserEmail}'." },
                    TraceId = HttpContext.TraceIdentifier
                };
                return NotFound(errorResponse);
            }

            var result =  await _userManager.ChangePasswordAsync(user, passwordRequest.OldPassword, passwordRequest.NewPassword);
            if (!result.Succeeded)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Password reset failed",
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }
            return Ok(new { message = "Password reset successful" });
        }
    }

}
