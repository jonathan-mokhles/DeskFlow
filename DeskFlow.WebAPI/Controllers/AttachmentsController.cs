using DeskFkow.Core.DTOs.AttachementDTOs;
using DeskFkow.Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

namespace DeskFkow.WebAPI.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId}/[controller]")]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ITicketAttachmentService _ticketAttachmentService;

        public AttachmentsController(IAuthorizationService authorizationService, ITicketAttachmentService ticketAttachmentService)
        {
            _authorizationService = authorizationService;
            _ticketAttachmentService = ticketAttachmentService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AttachementResponseDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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


        [HttpGet("{id:int}/download")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

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


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
