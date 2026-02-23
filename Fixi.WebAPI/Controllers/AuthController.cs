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
        [ProducesResponseType(typeof(ApiErrorResponse),StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Invalid registration data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (existingUser != null)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "email already exists",
                    Errors = new List<string> { $"A user with the email '{registerDTO.Email}' already exists." },
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }

            if (!await _roleManager.RoleExistsAsync(registerDTO.Role))
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Invalid role",
                    Errors = new List<string> { $"Role '{registerDTO.Role}' does not exist." },
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
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
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "User registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
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
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
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
                    Message = "Invalid credentials",
                    Errors = new List<string> { "Email or password is incorrect." },
                    TraceId = HttpContext.TraceIdentifier
                });
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
                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid token",
                        Errors = new List<string> { "Token does not contain a valid email claim." },
                        TraceId = HttpContext.TraceIdentifier
                    });
                }
            ApplicationUser? user = await _userManager.FindByEmailAsync(userEmail);
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
        /// <param name="passwordRequest">An object containing the user's email address, current password, and new password. All fields are required.
        /// The email must correspond to an existing user.</param>
        /// <returns>An HTTP 200 OK response if the password is successfully reset; otherwise, an HTTP 400 Bad Request response
        /// with error details.</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
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
                return BadRequest(errorResponse);
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

        [HttpGet("test")]
        public IActionResult test()
        {
            _logger.LogInformation("Test endpoint hit");
            return Ok("test");
        }
    }

}
