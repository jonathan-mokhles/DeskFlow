using DeskFlow.Core.DTOs.shared;
using DeskFlow.Core.DTOs.UsersDTOs;
using DeskFlow.Core.Enums;
using DeskFlow.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeskFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;


        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        /// <summary>
        /// Registers a new user account with the specified registration details. Accessible only to users in the Admin
        /// role.
        /// </summary>
        /// <remarks>This action requires authentication and is restricted to users with the Admin role.</remarks>
        /// <param name="registerDTO">An object containing the registration information for the new user, including email, full name, department,
        /// phone number, and password. All fields are required.</param>
        /// <returns>A 201 Created result if the user is successfully registered; otherwise, a 400 Bad Request result containing
        /// validation errors or identity errors.</returns>
        [HttpPost]
        [Authorize(policy: "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> register([FromBody]RegisterUserDTO registerDTO)
        {
             await _userService.RegisterUserAsync(registerDTO);
            _logger.LogInformation("User {Email} registered successfully", registerDTO.Email);
            return CreatedAtAction(nameof(register), new { email = registerDTO.Email }, null);
        }

        /// <summary>
        /// Update user deatails
        /// </summary>
        /// <remarks>Accessible only for Admin users.</remarks>
        /// <param name="id"></param>
        /// <param name="updateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(policy: "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDTO updateDTO)
        {
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

        /// <summary>
        /// Deletes the user with the specified identifier.
        /// </summary>
        /// <remarks>Accissable only for admin role.</remarks>
        /// <param name="id">The unique identifier of the user to delete. Cannot be null or empty.</param>
        /// <returns>A 204 No Content response if the user is deleted successfully; otherwise, a 400 Bad Request response
        /// containing error details.</returns>
        [HttpDelete("{id}")]
        [Authorize(policy: "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeactivateUserAsync(id);

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

        /// <summary>
        /// Get user deatails by id.
        /// </summary>
        /// <remarks>Accessible for Admin, Manager and the user himself. Managers can only access users in their department.</remarks>
        /// <param name="id">The unique identifier of the user to retrieve. Cannot be null or empty.</param>
        /// <returns>A 200 OK response containing the user details if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
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
            if (id == userid || user.Role == nameof(RoleEnum.Admin) || (user.Role == nameof(RoleEnum.Manager) && user.DepartmentId == deptId)) {
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
                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);

            }

        }


        /// <summary>
        /// Retrives a list of users based on the specified query parameter.
        /// </summary>
        /// <remarks>Accessible for Admin and Manager roles. Managers can only access users in their department.</remarks>
        /// <param name="query">The query parameters for filtering and pagination.</param>
        /// <returns>A 200 OK response containing the list of users if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet]
        [Authorize(policy: "AdminOrManager")]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers([FromQuery] UsersQueryParameters query)
        {
            if (User.IsInRole(nameof(RoleEnum.Manager)))
            {
                query.DepartmentId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "DeptId")?.Value ?? "0");
            }
            var users = await _userService.GetAllUsersAsync(query);
            return Ok(users);
        }
    }
}
