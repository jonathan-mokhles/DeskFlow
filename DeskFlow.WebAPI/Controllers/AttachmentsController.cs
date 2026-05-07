using DeskFlow.Core.DTOs.AttachementDTOs;
using DeskFlow.Core.DTOs.shared;
using DeskFlow.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

namespace DeskFlow.WebAPI.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing file attachments associated with a specific ticket. Supports retrieving,
    /// uploading, downloading, and deleting attachments for authorized users.
    /// </summary>
    /// <remarks>All actions require the user to be authorized as a manager, reporter, or assigned user for
    /// the specified ticket. Endpoints are accessible under the route pattern 'api/tickets/{ticketId}/attachments'.
    /// File uploads must use multipart/form-data. Responses include appropriate status codes for authorization and
    /// resource existence.</remarks>
    [ApiController]
    [Route("api/tickets/{ticketId}/[controller]")]
    [Authorize]

    public class AttachmentsController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ITicketAttachmentService _ticketAttachmentService;
        /// <summary>
        /// Initializes a new instance of the AttachmentsController class with the specified authorization and ticket
        /// attachment services.
        /// </summary>
        /// <param name="authorizationService">The service used to authorize user actions for attachment-related operations. Cannot be null.</param>
        /// <param name="ticketAttachmentService">The service responsible for managing ticket attachments. Cannot be null.</param>
        public AttachmentsController(IAuthorizationService authorizationService, ITicketAttachmentService ticketAttachmentService)
        {
            _authorizationService = authorizationService;
            _ticketAttachmentService = ticketAttachmentService;
        }

        /// <summary>
        /// Retrieves the list of attachments associated with the specified ticket.
        /// </summary>
        /// <remarks>Returns status code 200 (OK) with the list of attachments if successful. Returns 403
        /// (Forbidden) if the user is not authorized to access the ticket, or 404 (Not Found) if the ticket does not
        /// exist.</remarks>
        /// <param name="ticketId">The unique identifier of the ticket for which to retrieve attachments.</param>
        /// <returns>An <see cref="IActionResult"/> containing a collection of attachment details if the request is authorized
        /// and the ticket exists; otherwise, a status code indicating the error.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AttachementResponseDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden , Type =typeof(ApiErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type =typeof(ApiErrorResponse))]
       
        public async Task<IActionResult> GetAttachments([FromRoute] int ticketId)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ticketId, "ManagerOrReporterOrAssignedTo");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var attachments = await _ticketAttachmentService.GetAttachmentsByTicketIdAsync(ticketId);
            return Ok(attachments);
        }

        /// <summary>
        /// Uploads a new attachment file for the specified ticket.
        /// </summary>
        /// <param name="ticketId">The unique identifier of the ticket to which the attachment will be uploaded.</param>
        /// <param name="fileDTO">The file data transfer object containing the file to be uploaded.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the upload operation.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiErrorResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type =typeof(ApiErrorResponse))]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAttachment([FromRoute] int ticketId, [FromForm] UploadFileDTO fileDTO)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ticketId, "ManagerOrReporterOrAssignedTo");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (fileDTO.File == null || fileDTO.File.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }
            await _ticketAttachmentService.UploadAttachmentAsync(ticketId, fileDTO.File, userId);
            return Ok();
        }

        /// <summary>
        /// Downloads the specified attachment file associated with a ticket.
        /// </summary>
        /// <remarks>The user must have appropriate permissions (Manager, Reporter, or AssignedTo) for the
        /// specified ticket to download the attachment. Returns a 403 status code if the user is not authorized, or a
        /// 404 status code if the attachment does not exist.</remarks>
        /// <param name="ticketId">The unique identifier of the ticket to which the attachment belongs.</param>
        /// <param name="id">The unique identifier of the attachment to download.</param>
        /// <returns>A file result containing the attachment if found and authorized; otherwise, a 403 Forbidden or 404 Not Found
        /// response.</returns>
        [HttpGet("{id:int}/download")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
        [ProducesResponseType(StatusCodes.Status403Forbidden,Type = typeof(ApiErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type =typeof(ApiErrorResponse))]

        public async Task<IActionResult> DownloadAttachment([FromRoute] int ticketId, [FromRoute] int id)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ticketId, "ManagerOrReporterOrAssignedTo");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var fileData = await _ticketAttachmentService.DownloadAttachmentAsync(ticketId, id);
            return File(fileData.FileStream, fileData.MimeType, fileData.FileName);
        }

        /// <summary>
        /// Deletes the specified attachment from the given ticket.
        /// </summary>
        /// <remarks>The user must have appropriate permissions (Manager, Reporter, or AssignedTo) to
        /// delete the attachment. If the user lacks the required authorization, the operation is forbidden.</remarks>
        /// <param name="ticketId">The unique identifier of the ticket from which the attachment will be deleted.</param>
        /// <param name="id">The unique identifier of the attachment to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the delete operation. Returns status code 200 (OK)
        /// if the attachment was deleted successfully, 403 (Forbidden) if the user is not authorized, or 404 (Not
        /// Found) if the ticket or attachment does not exist.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden,Type = typeof(ApiErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type =typeof(ApiErrorResponse))]
        public async Task<IActionResult> DeleteAttachment([FromRoute] int ticketId, [FromRoute] int id)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ticketId, "ManagerOrReporterOrAssignedTo");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            await _ticketAttachmentService.DeleteAttachmentAsync(ticketId, id);
            return Ok();
        }
    }
}
