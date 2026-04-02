using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.CommentDTOs;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.ServicesContracts;
using Fixi.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixi.WebAPI.Controllers
{
    [Route("api/tickets/{ticketId}/comments")]
    [ApiController]
    public class TicketCommentController : ControllerBase
    {
        ITicketCommentsService _CommentsService;
        ITicketRepository _ticketRepository;
        IAuthorizationService _authorizationService;


        public TicketCommentController(ITicketCommentsService ticketCommentsService, ITicketRepository ticketRepository, IAuthorizationService authorizationService)
        {
            _CommentsService = ticketCommentsService;
            _ticketRepository = ticketRepository;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddComment(int ticketId, CommentCreateDTO comment)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
            if (ticket is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Ticket not found.",
                    Errors = new List<string> { "No ticket with the specified ID." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            var result = await _authorizationService.AuthorizeAsync(User, ticket, "ManagerOrReporterOrAssignedTo");
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Message = "You do not have permission to add comment.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    Message = "Invalid comment data",
                    TraceId = HttpContext.TraceIdentifier,
                    
                });
            }

            if(ticketId != comment.TicketId)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Errors = new List<string> { "Ticket ID in URL does not match Ticket ID in comment data" },
                    Message = "Invalid comment data",
                    TraceId = HttpContext.TraceIdentifier,
                });
            }

            await _CommentsService.AddCommentToTicketAsync(comment);
            return Ok();
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<CommentResponseDTO>),StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
            if (ticket is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Ticket not found.",
                    Errors = new List<string> { "No ticket with the specified ID." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            var result = await _authorizationService.AuthorizeAsync(User, ticket, "ManagerOrReporterOrAssignedTo");
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Message = "You do not have permission to add comment.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    Message = "Invalid comment data",
                    TraceId = HttpContext.TraceIdentifier,

                });
            }


            var tickets = await _CommentsService.GetCommentsForTicketAsync(ticketId);

            return  Ok(tickets);
        }


        [HttpDelete("{commentId}")]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComment(int ticketId, int commentId)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
            if (ticket is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Ticket not found.",
                    Errors = new List<string> { "No ticket with the specified ID." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            var result = await _authorizationService.AuthorizeAsync(User, ticket, "ManagerOrReporterOrAssignedTo");
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Message = "You do not have permission to delete comment.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }



            await _CommentsService.DeleteCommentFromTicketAsync(commentId);
            return Ok();
        }



    }
}
