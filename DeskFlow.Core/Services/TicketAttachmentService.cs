using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.DTOs.AttachementDTOs;
using DeskFlow.Core.Exceptions;
using DeskFlow.Core.Mappings;
using DeskFlow.Core.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Services
{
    public class TicketAttachmentService : ITicketAttachmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public TicketAttachmentService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<(Stream FileStream, string MimeType, string FileName)> DownloadAttachmentAsync(int ticketId, int attachmentId)
        {
            var attachment = await _unitOfWork.TicketAttachment.GetByIdAsync(attachmentId);
            if (attachment == null || attachment.TicketId != ticketId)
            {
                throw new NotFoundException("Attachment not found for the given ticket.");
            }

            var fileData = await _fileStorageService.GetFileAsync(attachment.FilePath);
            return (fileData.FileStream, fileData.MimeType, attachment.FileName);
        }

        public async Task<IEnumerable<AttachementResponseDTO>> GetAttachmentsByTicketIdAsync(int ticketId)
        {
            return await _unitOfWork.TicketAttachment.GetByticketIdAsync(ticketId);
        }

        public async Task UploadAttachmentAsync(int ticketId, IFormFile file, string userId)
        {
            string filePath = await _fileStorageService.SaveFileAsync(file, ticketId);
            var attachment = file.ToEntity(ticketId, filePath, userId);

            await _unitOfWork.TicketAttachment.CreateAsync(attachment);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAttachmentAsync(int ticketId, int attachmentId)
        {
            var attachment = await _unitOfWork.TicketAttachment.GetByIdAsync(attachmentId);
            if (attachment == null || attachment.TicketId != ticketId)
            {
                throw new NotFoundException("Attachment not found for the given ticket.");
            }
            await _fileStorageService.DeleteFileAsync(attachment.FilePath);
            await _unitOfWork.TicketAttachment.DeleteAsync(attachment);
            await _unitOfWork.CommitAsync();
        }


    }
}
