using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using Fixi.Core.Mappings;
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
    [Authorize]
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

        /// <summary>
        /// open new ticket.
        /// </summary>
        /// <param name="createTicketDTO"></param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateTicket([FromBody] CreateTicketDTO createTicketDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Ticket createdTicket = await _ticketService.CreateTicketAsync(createTicketDTO, userId);
            var responseDTO = createdTicket.ToFullResponseDto();
            return CreatedAtAction(nameof(GetTicketById), new { id = createdTicket.Id }, responseDTO);
        }

        /// <summary>
        /// Retrive ticket by Id.
        /// </summary>
        /// <remarks> Only the reporter, assigned technician, manager of the department or admin can access the ticket details.</remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TicketResponseDTO>> GetTicketById(int id)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, id, "ManagerOrReporterOrAssignedTo");
            if(!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden,new ApiErrorResponse
                {
                    Message = "You do not have permission to view this ticket.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return Ok(ticket);
        }

        /// <summary>
        /// Retrives all tickets with pagination and optional filtering by status, priority, department, category, reporter or assigned technician.
        /// </summary>
        /// <param name="queryParams">The query parameters for filtering and pagination.</param>
        /// <returns>A list of tickets matching the specified criteria.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<TicketResponseDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetAllTickets([FromQuery] TicketQueryParams queryParams)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            string userid = User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

            var tickets = await _ticketService.GetAllTicketsAsync(queryParams, GetUserClaims());
            return Ok(tickets);
        }

        /// <summary>
        /// Udpate all ticket details except status, assigned technician and priority.
        /// </summary>
        /// <remarks> Only the reporter, assigned technician, manager of the department or admin can update the ticket details.</remarks>
        /// <param name="id">The unique identifier of the ticket to update. Cannot be null or empty.</param>
        /// <param name="updateTicketDTO">An object containing the updated ticket details. All fields are required.</param>
        /// <returns>A 204 No Content response if the ticket is updated successfully; otherwise, a 400 Bad Request response containing error details.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
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
            await _ticketService.UpdateTicketAsync(updateTicketDTO, GetUserClaims());
            return NoContent();
        }


        /// <summary>
        /// Updates the status of the specified ticket to a new value.
        /// </summary>
        /// <remarks>This operation requires the caller to have appropriate permissions to modify the
        /// ticket. </remarks>
        /// <param name="ticketId">The unique identifier of the ticket to update.</param>
        /// <param name="statusDTO">An object containing the new status value and an optional comment.</param>
        /// <returns>A result indicating the outcome of the operation. Returns 204 No Content if the update is successful, or 400
        /// Bad Request if the new status value is invalid.</returns>
        [HttpPatch("{ticketId}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateTicketStatus(int ticketId, TicketUpdateStatusDTO statusDTO)
        {
            if(statusDTO.NewStatus < 0 || statusDTO.NewStatus > 6)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid status value. Status must be between 0 and 6.",
                    Errors = new List<string> { "Status value out of range." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            await _ticketService.UpdateTicketStatus(ticketId, statusDTO, GetUserClaims());
            return NoContent();
        }


        /// <summary>
        /// Assigns a technician to the specified support ticket.
        /// </summary>
        /// <remarks>This action requires the caller to have the ManagerOrTechnician authorization policy.</remarks>
        /// <param name="ticketId">The unique identifier of the support ticket to which the technician will be assigned.</param>
        /// <param name="newTechnicianId">The unique identifier of the technician to assign to the ticket. Cannot be null or empty.</param>
        /// <returns>A result indicating the outcome of the operation. Returns a 204 No Content response if the assignment is
        /// successful.</returns>
        [HttpPatch("{ticketId}/assign")]
        [Authorize(policy: "ManagerOrTechnician")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]

        public async Task<ActionResult> AssignTechnician(int ticketId, string newTechnicianId)
        {
            await _ticketService.AssignTechnician(ticketId, newTechnicianId, GetUserClaims());
            return NoContent();
        }


        /// <summary>
        /// Updates the priority of the specified ticket.
        /// </summary>
        /// <param name="ticketId">The unique identifier of the ticket to update.</param>
        /// <param name="newPriority">The new priority value to assign to the ticket. Must be an integer between 1 and 3, inclusive.</param>
        /// <returns>A result indicating the outcome of the operation. Returns 204 No Content if the update is successful, or 400
        /// Bad Request if the new priority value is invalid.</returns>
        [HttpPatch("{ticketId}/priority")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
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

            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
            if(ticket is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Ticket not found.",
                    Errors = new List<string> { "No ticket with the specified ID." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ticket.DepartmentId, "ManagerOrAdmin");
            if(!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse
                {
                    Message = "You do not have permission to change this ticket's priority.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            await _ticketService.UpdateTicketPriority(ticket, newPriority, User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            return NoContent();
        }

        /// <summary>
        /// delete ticket of specidied Id
        /// </summary>
        /// <remarks>Admin only can delete tickets.</remarks>
        /// <param name="id">The unique identifier of the ticket to delete.</param>
        /// <returns>A result indicating the outcome of the operation. Returns 204 No Content if the deletion is successful, or 400 Bad Request if the ticket cannot be deleted.</returns>
        [HttpDelete("{id}")]
        [Authorize(policy: "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            string userid = User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            await _ticketService.DeleteTicket(id, userid);
            return NoContent();
        }

        /// <summary>
        /// Retrieves the audit history for a specified ticket.
        /// </summary>
        /// <remarks>Returns a 404 response if the ticket does not exist, or a 401 response if the caller
        /// does not have permission to view the ticket's history.</remarks>
        /// <param name="ticketId">The unique identifier of the ticket for which to retrieve the audit history. Must be a valid ticket ID.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing a collection of <see cref="TicketAuditHistoryDTO"/> objects
        /// representing the ticket's audit history if found and authorized; otherwise, an error response indicating the
        /// reason for failure.</returns>
        [HttpGet("{ticketId}/history")]
        [ProducesResponseType(typeof(IEnumerable<TicketAuditHistoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest )]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden )]
        public async Task<ActionResult<IEnumerable<TicketAuditHistoryDTO>>> GetTicketHistory(int ticketId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, ticketId, "ManagerOrReporterOrAssignedTo");
            if(!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse
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
            string role = User.FindFirstValue(ClaimTypes.Role)!;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            string deptId = User.FindFirstValue("DeptId")!;
            int.TryParse(deptId, out var parsedDeptId);
            return new UserClaims
            {
                UserId = userId,
                Role = role,
                DeptId = parsedDeptId,
            };
        }
    }
}
