using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.DTOs.AccountDTOs;
using Fixi.Core.DTOs.shared;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace Fixi.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwt;
        private readonly ILogger<AuthController> _logger;


        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtService jwtService, RoleManager<IdentityRole> roleManager, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwtService;
            _roleManager = roleManager;
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
            if (!ModelState.IsValid)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                    {
                        Message = "Invalid login data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                        TraceId = HttpContext.TraceIdentifier
                    };
                return BadRequest(errorResponse);
            }

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

            AuthResponseDTO authResponse = await _jwt.GenerateToken(user);
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
            if (!ModelState.IsValid)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }


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

                var response = await _jwt.GenerateToken(user);
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
        public async Task<IActionResult> resetPassword(ResetPasswordRequestDTO passwordRequest)
        {
            if (!ModelState.IsValid)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }

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
