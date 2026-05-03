using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DeskFkow.Core.ServicesContracts;



using Microsoft.AspNetCore.Authorization;
using DeskFkow.Core.DTOs.SLADTOs;
using DeskFkow.Core.DTOs.shared;
using DeskFkow.Core.Domain.Entity;

namespace DeskFkow.WebAPI.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing Service Level Agreement (SLA) resources. 
    /// </summary>
    /// <remarks> Access to these endpoints is restricted to users with the AdminOnly policy.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(policy: "AdminOnly")]
    public class SLAController : ControllerBase
    {
        private readonly ISLAService _slaService;

        public SLAController(ISLAService slaService)
        {
            _slaService = slaService;
        }

        /// <summary>
        /// Create new SLA 
        /// </summary>
        /// <param name="createDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateSLA(SLACreateDTO createDTO)
        {
            int id = await _slaService.CreateSLA(createDTO);

            return CreatedAtAction(nameof(GetSLAById), new { id });
        }

        /// <summary>
        /// Retrieves all service level agreements (SLAs).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SLASetting>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllSLA()
        {
            return Ok(await _slaService.GetAllSLA());
        }

        /// <summary>
        /// Retrieves a specific service level agreement (SLA) by its Id.
        /// </summary>
        /// <param name="id"> SLA Id</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SLASetting),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSLAById(int id)
        {
            var sla = await _slaService.GetSLAById(id);
            if (sla == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "SLA not found",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            return Ok(sla);
        }

        /// <summary>
        /// Updates the specified SLA setting with new values.
        /// </summary>
        /// <param name="id">The unique identifier of the SLA setting to update. Must match the Id property of the provided SLA setting.</param>
        /// <param name="sLA">The updated SLA setting values. The Id property must match the id parameter.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateSLA(int id, SLASetting sLA)
        {
            if (id != sLA.Id)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "ID mismatch",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            var existingSLA = await _slaService.GetSLAById(id);
            if (existingSLA == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "SLA not found",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            await _slaService.UpdateSLA(sLA);
            return NoContent();
        }

        /// <summary>
        /// Delete SLA by its Id
        /// </summary>
        /// <param name="id"> SLA Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSLA(int id)
        {
            var existingSLA = await _slaService.GetSLAById(id);
            if (existingSLA == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "SLA not found",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            await _slaService.DeleteSLA(id);
            return NoContent();
        }
    }
}
