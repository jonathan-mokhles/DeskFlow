using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.UsersDTOs;
using Fixi.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fixi.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly Logger<UsersController> _logger;


        public UsersController(UserService userService, Logger<UsersController> logger)
        {
            _userService = userService;
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
        [HttpPost]
        [Authorize(policy: "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<IActionResult> register(RegisterUserDTO registerDTO)
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

            await _userService.RegisterUserAsync(registerDTO);
            _logger.LogInformation("User {Email} registered successfully", registerDTO.Email);
            return CreatedAtAction(nameof(register), new { email = registerDTO.Email });
        }


        [HttpPut("{id}")]
        [Authorize(policy: "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Invalid update data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }

            if (id != updateDTO.Id)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "User ID mismatch",
                    Errors = new List<string> { "The user ID in the URL does not match the user ID in the request body." },
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }

            var result = await _userService.UpdateUserAsync(id, updateDTO);
            if (!result.Succeeded)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Failed to update user",
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }

            _logger.LogInformation("User {UserId} updated successfully", id);
            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(policy: "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.Succeeded)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Failed to delete user",
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }

            _logger.LogInformation("User {UserId} deleted successfully", id);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "User not found",
                    Errors = new List<string> { $"No user found with ID '{id}'." },
                    TraceId = HttpContext.TraceIdentifier
                };
                return NotFound(errorResponse);
            }

            var role = User.FindFirstValue(ClaimTypes.Role);
            string userid = User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

            int deptId = int.Parse(User.FindFirstValue("DeptId")!);
            if (id == userid || user.Role == "Admin" || (user.Role == "Manager" && user.DepartmentId == deptId)) {
                return Ok(user);
            }
            else
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message = "Unauthorized access",
                    Errors = new List<string> { "You do not have permission to access this user's information." },
                    TraceId = HttpContext.TraceIdentifier
                };
                return Unauthorized(errorResponse);
            }

        }


        [HttpGet]
        [Authorize(policy: "AdminOrManager")]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<IActionResult> GetAllUsers([FromQuery] UsersQueryParameters query)
        {
            if (User.IsInRole("Manager"))
            {
                query.DepartmentId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "DeptId")?.Value ?? "0");
            }
            var users = await _userService.GetAllUsersAsync(query);
            return Ok(users);
        }
    }
}
