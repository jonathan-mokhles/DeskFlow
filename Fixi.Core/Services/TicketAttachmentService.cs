using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.AttachementDTOs;
using Fixi.Core.Exceptions;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Services
{
    public class TicketAttachmentService : ITicketAttachmentService
    {
        private readonly ITicketAttachmentRepository _attachmentRepository;
        private readonly IFileStorageService _fileStorageService;

        public TicketAttachmentService(ITicketAttachmentRepository attachmentRepository, IFileStorageService fileStorageService)
        {
            _attachmentRepository = attachmentRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<(Stream FileStream, string MimeType, string FileName)> DownloadAttachmentAsync(int ticketId, int attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null || attachment.TicketId != ticketId)
            {
                throw new NotFoundException("Attachment not found for the given ticket.");
            }

            var fileData = await _fileStorageService.GetFileAsync(attachment.FilePath);
            return (fileData.FileStream, fileData.MimeType, attachment.FileName);
        }

        public async Task<IEnumerable<AttachementResponseDTO>> GetAttachmentsByTicketIdAsync(int ticketId)
        {
            return await _attachmentRepository.GetByticketIdAsync(ticketId);
        }

        public async Task UploadAttachmentAsync(int ticketId, IFormFile file, string userId)
        {
            string filePath = await _fileStorageService.SaveFileAsync(file, ticketId);
            var attachment = new TicketAttachment
            {
                TicketId = ticketId,
                FileName = file.FileName,
                FilePath = filePath,
                FileSize = file.Length,
                MimeType = file.ContentType,
                UploadedById = userId,
                UploadedDate = DateTime.UtcNow,
                

            
            };

             await _attachmentRepository.CreateAsync(attachment);

        }

        public async Task DeleteAttachmentAsync(int ticketId, int attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null || attachment.TicketId != ticketId)
            {
                throw new NotFoundException("Attachment not found for the given ticket.");
            }
            await _fileStorageService.DeleteFileAsync(attachment.FilePath);
            await _attachmentRepository.DeleteAsync(attachment);
        }


    }
}
