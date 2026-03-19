using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fixi.WebAPI.Controllers
{
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


        [HttpGet]
        public IActionResult GetAllDepartments()
        {
            var departments = _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            _departmentService.DeleteDepartmentAsync(id);
            return NoContent();

        }

        [HttpPost]
        public IActionResult AddDepartment(string name)
        {
            _departmentService.CreateDepartmentAsync(name);
            return NoContent();
        }
    }
}
