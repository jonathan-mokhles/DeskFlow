using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.DTOs.CommentDTOs;
using DeskFlow.Core.DTOs.shared;
using DeskFlow.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace DeskFlow.WebAPI.Controllers
{
    /// <summary>
    /// for managing comments related to tickets.
    /// </summary>
    [Route("api/tickets/{ticketId}/comments")]
    [ApiController]
    [Authorize]
    public class TicketCommentController : ControllerBase
    {
        ITicketCommentsService _CommentsService;
        IAuthorizationService _authorizationService;


        public TicketCommentController(ITicketCommentsService ticketCommentsService, IAuthorizationService authorizationService)
        {
            _CommentsService = ticketCommentsService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Adds a new comment to the specified ticket.
        /// </summary>
        /// <remarks>Admin, Depatment Manger, Assigned technician and Reported User only can comment</remarks>
        /// <param name="ticketId">The unique identifier of the ticket to which the comment will be added.</param>
        /// <param name="comment">The data for the comment to add, including content and associated ticket information. Must not be null, and
        /// the TicketId property must match the ticketId parameter.</param>
       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, CommentCreateDTO comment)
        {

            var result = await _authorizationService.AuthorizeAsync(User, ticketId, "ManagerOrReporterOrAssignedTo");
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse
                {
                    Message = "You do not have permission to add comment.",
                    Errors = new List<string> { "Forbidden access." },
                    TraceId = HttpContext.TraceIdentifier
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

        /// <summary>
        /// Retrive all comments related to a specific ticket by providing the ticket's unique identifier.
        /// </summary>
        /// <remarks>Admin, Depatment Manger, Assigned technician and Reported User only can access the comments of the ticket</remarks>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CommentResponseDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetComments(int ticketId)
        {

            var result = await _authorizationService.AuthorizeAsync(User, ticketId, "ManagerOrReporterOrAssignedTo");
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse
                {
                    Message = "You do not have permission to add comment.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            var tickets = await _CommentsService.GetCommentsForTicketAsync(ticketId);

            return  Ok(tickets);
        }

        /// <summary>
        /// Delete a comment from a ticket by providing the ticket's unique identifier and the comment's unique identifier.
        /// </summary>
        /// <remarks>Only the comment's author or an admin can delete a comment.</remarks>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete("{commentId}")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
            CommentResponseDTO? comment = await _CommentsService.GetCommentByIdAsync(commentId);
            if(comment is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Comment not found.",
                    Errors = new List<string> { "No comment with the specified ID." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            if (comment.UserID != userId )
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse
                {
                    Message = "You do not have permission to delete comment.",
                    Errors = new List<string> { "Unauthorized access." },
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            await _CommentsService.DeleteCommentFromTicketAsync(commentId);
            return NoContent();
        }



    }
}
