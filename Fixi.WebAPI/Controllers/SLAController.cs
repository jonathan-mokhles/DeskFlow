using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fixi.Core.ServicesContracts;



using Microsoft.AspNetCore.Authorization;
using Fixi.Core.DTOs.SLADTOs;
using Fixi.Core.DTOs.shared;
using Fixi.Core.Domain.Entity;

namespace Fixi.WebAPI.Controllers
{
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult> CreateSLA(SLACreateDTO createDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            int id = await _slaService.CreateSLA(createDTO);

            return CreatedAtAction(nameof(GetSLAById), new { id });
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult> GetAllSLA()
        {
            return Ok(await _slaService.GetAllSLA());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
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


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
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


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public IActionResult DeleteSLA(int id)
        {
            var existingSLA = _slaService.GetSLAById(id).Result;
            if (existingSLA == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "SLA not found",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            _slaService.DeleteSLA(id);
            return NoContent();
        }
    }
}
