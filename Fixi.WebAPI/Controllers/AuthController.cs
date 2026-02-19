using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.DTOs.AccountDTOs;
using Fixi.Core.Services;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Win32;
using System.Security.Claims;
using System.Threading.Tasks;

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
        /// Registers a new user account with the specified registration details. Accessible only to users in the Admin
        /// role.
        /// </summary>
        /// <remarks>This action requires authentication and is restricted to users with the Admin role.
        /// The user is assigned to a role upon successful registration. The request body must provide valid
        /// registration data; otherwise, the request will fail validation.</remarks>
        /// <param name="registerDTO">An object containing the registration information for the new user, including email, full name, department,
        /// phone number, and password. All fields are required.</param>
        /// <returns>A 201 Created result if the user is successfully registered; otherwise, a 400 Bad Request result containing
        /// validation errors or identity errors.</returns>
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            if (!await _roleManager.RoleExistsAsync(registerDTO.Role))
            {
                return BadRequest(new { message = $"Role '{registerDTO.Role}' does not exist" });
            }

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                FullName = registerDTO.FullName,
                DepartmentId = registerDTO.DepartmentId,
                PhoneNumber = registerDTO.phone,

            };

            var result = await _userManager.CreateAsync(newUser, registerDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(newUser, registerDTO.Role);
            _logger.LogInformation("User {Email} registered successfully", registerDTO.Email);
            return CreatedAtAction(nameof(register), new { email = newUser.Email });


        }

        /// <summary>
        /// Authenticates a user with the provided credentials and returns a JWT token if successful.
        /// </summary>
        /// <remarks>This endpoint does not require authentication. The response includes a JWT token on
        /// successful authentication, which can be used for subsequent authorized requests. Returns HTTP 401 if the
        /// credentials are invalid, or HTTP 400 if the input model is invalid.</remarks>
        /// <param name="loginDTO">The login credentials, including email and password, used to authenticate the user. Cannot be null. The
        /// model must be valid.</param>
        /// <returns>An <see cref="OkObjectResult"/> containing an <see cref="AuthResponseDTO"/> with the authentication token if
        /// the credentials are valid; otherwise, an <see cref="UnauthorizedObjectResult"/> indicating invalid
        /// credentials or a <see cref="BadRequestObjectResult"/> if the input model is invalid.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", loginDTO.Email);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Failed login attempt for user: {UserId}", user.Id);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            AuthResponseDTO authResponse = await _jwt.GenerateToken(user);
            _logger.LogInformation("User {Email} logged in successfully", loginDTO.Email);
            return Ok(authResponse);

        }

        /// <summary>
        /// Handles a request to refresh a JWT access token using a valid refresh token.
        /// </summary>
        /// <remarks>This endpoint does not require authentication. The client must provide both the
        /// expired access token and a valid refresh token. If the refresh token is invalid, expired, or does not match
        /// the user, the request will be rejected.</remarks>
        /// <param name="request">The refresh token request containing the expired access token and the associated refresh token. Must not be
        /// null and must contain valid token values.</param>
        /// <returns>An <see cref="IActionResult"/> that represents the result of the refresh operation. Returns a 200 OK
        /// response with a new access token and refresh token if successful; otherwise, returns a 400 Bad Request
        /// response with an error message.</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> refreshToken(RefreshTokenRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
               return BadRequest(new { message = "Invalid request" });
            }

            try
            {
                var principal = _jwt.GetPrincipalFromExpiredToken(request.Token);
                if(principal == null)
                {
                    return BadRequest(new { message = "Invalid token" });
                }

                string? userEmail =  principal.FindFirstValue(ClaimTypes.Email);

                ApplicationUser? user = await _userManager.FindByEmailAsync(userEmail);
                if(user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Invalid token or refresh token" });
                }
                var response = await _jwt.GenerateToken(user);
                return Ok(response);
            }
            catch (Exception ex) { 
                return BadRequest(new { message = "Invalid token", details = ex.Message });
            }
        }

        /// <summary>
        /// Resets the password for a user identified by the provided email address.
        /// </summary>
        /// <param name="passwordRequest">An object containing the user's email address, current password, and new password. All fields are required.
        /// The email must correspond to an existing user.</param>
        /// <returns>An HTTP 200 OK response if the password is successfully reset; otherwise, an HTTP 400 Bad Request response
        /// with error details.</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> resetPassword(ResetPasswordRequestDTO passwordRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ApplicationUser? user = await _userManager.FindByEmailAsync(passwordRequest.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User with this email does not exist" });
            }
            var result =  await _userManager.ChangePasswordAsync(user, passwordRequest.OldPassword, passwordRequest.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "Password reset successful" });
        }

        [HttpGet("test")]
        public IActionResult test()
        {
            _logger.LogInformation("Test endpoint hit");
            return Ok("test");
        }
    }

}
