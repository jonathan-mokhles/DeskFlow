using DeskFlow.Core.Domain.Entity;
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
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Retrive all the departments
        /// </summary>
        /// <returns>list of departments</returns>
        [ProducesResponseType(typeof(IEnumerable<Department>),StatusCodes.Status200OK)]
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
        /// <param name="name">The name of the department to create. Cannot be null or empty.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddDepartment(string name)
        {
            var department = await _departmentService.CreateDepartmentAsync(name);
            return CreatedAtAction(nameof(GetAllDepartments), new { name = department.Name }, department);
        }
    }
}
