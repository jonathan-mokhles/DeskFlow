using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.DepartmentDTO;
using DeskFlow.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeskFlow.WebAPI.Controllers
{
    /// <summary>
    /// Represents an API controller that manages department resources.
    /// </summary>
    /// <remarks>Access to this controller is restricted to users with the AdminOnly policy</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(policy: "AdminOnly")]
    
    public class DepartmentController : ControllerBase
    {
        IDepartmentService _departmentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentController"/> class with the specified department service.
        /// </summary>
        /// <param name="departmentService">The service for managing departments.</param>
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Retrive all the departments
        /// </summary>
        /// <returns>list of departments</returns>
        [ProducesResponseType(typeof(IEnumerable<DepartmentResponseDTO>),StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        /// <summary>
        /// Deletes the department with Id.
        /// </summary>
        /// <param name="id">The Id of the department to delete.</param>
        /// <returns>An HTTP 204 No Content response if the department was successfully deleted.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return NoContent();

        }

        /// <summary>
        /// Creates a new department with the specified name.
        /// </summary>
        /// <param name="createDTO">The DTO containing the details of the department to create. Cannot be null.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentCreateDTO createDTO)
        {
            var department = await _departmentService.CreateDepartmentAsync(createDTO);
            return CreatedAtAction(nameof(GetAllDepartments), department);
        }
    }
}
