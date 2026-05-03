using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.DTOs.AttachementDTOs;
using Microsoft.AspNetCore.Http;
using System;

namespace DeskFkow.Core.Mappings
{
    public static class AttachmentMappings
    {
        public static TicketAttachment ToEntity(this IFormFile file, int ticketId, string filePath, string userId)
        {
            return new TicketAttachment
            {
                TicketId = ticketId,
                FileName = file.FileName,
                FilePath = filePath,
                FileSize = file.Length,
                MimeType = file.ContentType,
                UploadedById = userId,
                UploadedDate = DateTime.UtcNow
            };
        }

        public static AttachementResponseDTO ToResponseDto(this TicketAttachment attachment)
        {
            return new AttachementResponseDTO
            {
                Id = attachment.Id,
                TicketId = attachment.TicketId,
                FileName = attachment.FileName,
                FileSize = attachment.FileSize,
                MimeType = attachment.MimeType,
                UploadedById = attachment.UploadedById,
                UploadedByName = attachment.UploadedBy?.FullName ?? string.Empty,
                UploadedDate = attachment.UploadedDate
            };
        }
    }
}
