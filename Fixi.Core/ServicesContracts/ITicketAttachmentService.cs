using Fixi.Core.DTOs.AttachementDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public interface ITicketAttachmentService
    {
        public Task<IEnumerable<AttachementResponseDTO>> GetAttachmentsByTicketIdAsync(int ticketId);
        public Task UploadAttachmentAsync(int ticketId, IFormFile file, string userId);
        public Task<(Stream FileStream, string MimeType, string FileName)> DownloadAttachmentAsync(int ticketId, int attachmentId);

        public Task DeleteAttachmentAsync(int ticketId, int attachmentId);
    }
}
