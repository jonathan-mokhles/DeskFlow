using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fixi.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        ITicketService _ticketService;
        IAuthorizationService _authorizationService;
        ITicketRepository _ticketRepository;

        public TicketsController(ITicketService ticketService, IAuthorizationService authorizationService, ITicketRepository ticketRepository )
        {
            _ticketService = ticketService;
            _authorizationService = authorizationService;
            _ticketRepository = ticketRepository;
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult> CreateTicket(CreateTicketDTO createTicketDTO)
        {
            Ticket createdTicket = await _ticketService.CreateTicketAsync(createTicketDTO);


            return CreatedAtAction(nameof(GetTicketById), new { id = createdTicket.Id }, createdTicket);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<TicketResponseDTO>> GetTicketById(int id)
        {
            TicketDTO ticketDto = await _ticketRepository.GetTicketAsync(id);
            if(ticketDto is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Ticket not found.",
                    Errors = new List<string> { "No ticket with the specified ID." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ticketDto, "ManagerOrReporterOrAssignedTo");
            if(!authorizationResult.Succeeded)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Message = "You do not have permission to view this ticket.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return Ok(ticket);
        }


        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<TicketResponseDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetAllTickets([FromQuery] TicketQueryParams queryParams)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            string userid = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var tickets = await _ticketService.GetAllTicketsAsync(queryParams, GetUserClaims());
            return Ok(tickets);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult> UpdateTicket(int id, UpdateTicketDTO updateTicketDTO)
        {
            if(id != updateTicketDTO.Id)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Ticket ID is wrong.",
                    Errors = new List<string> { "ID mismatch." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            if(!ModelState.IsValid )
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            await _ticketService.UpdateTicketAsync(updateTicketDTO, GetUserClaims());
            return NoContent();
        }



        [HttpPatch("{ticketId}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult> UpdateTicketStatus(int ticketId, int newStatus)
        {
            if(newStatus < 0 || newStatus > 6)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid status value. Status must be between 0 and 6.",
                    Errors = new List<string> { "Status value out of range." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            await _ticketService.UpdateTicketStatus(ticketId, (TicketStatus)newStatus, GetUserClaims());
            return NoContent();
        }



        [HttpPatch("{ticketId}/assign")]
        [Authorize(Roles = "Manager,Technician")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult> AssignTechnician(int ticketId, string newTechnicianId)
        {
            await _ticketService.AssignTechnician(ticketId, newTechnicianId, GetUserClaims());
            return NoContent();
        }



        [HttpPatch("{ticketId}/priority")]
        [Authorize( policy: "ManagerOmly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult> UpdateTicketPriority(int ticketId, int newPriority)
        {
            if(newPriority < 1 || newPriority > 3)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid priority value. Priority must be between 1 and 3.",
                    Errors = new List<string> { "Priority value out of range." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            await _ticketService.UpdateTicketPriority(ticketId, newPriority,GetUserClaims());
            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(policy: "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            string userid = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            await _ticketService.DeleteTicket(id, userid);
            return NoContent();
        }

        [HttpGet("{ticketId}/history")]
        [ProducesResponseType(typeof(IEnumerable<TicketAuditHistoryDTO>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<IEnumerable<TicketAuditHistoryDTO>>> GetTicketHistory(int ticketId)
        {
            TicketDTO ticket = await _ticketRepository.GetTicketAsync(ticketId);
            if(ticket is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Ticket not found.",
                    Errors = new List<string> { "No ticket with the specified ID." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            var result = await _authorizationService.AuthorizeAsync(User, ticketId, "ManagerOrReporterOrAssignedTo");
            if(!result.Succeeded)
            {
                return Unauthorized( new ApiErrorResponse
                {
                    Message = "You do not have permission to view this ticket's history.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            var history = await _ticketService.GetTicketHistoryAsync(ticketId);
            return Ok(history);
        }


        private UserClaims GetUserClaims()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            string userid = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            
            string deptId = User.FindFirstValue("DeptId");
            int.TryParse(deptId, out var parsedDeptId);
            return new UserClaims
            {
                UserId = userid,
                Role = role,
                DeptId = parsedDeptId,
            };
        }
    }
}
