using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.AttachementDTOs;
using Fixi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Fixi.Infrastructure.Repositories
{
    public class TicketAttachmentRepository : ITicketAttachmentRepository
    {
        private readonly ApplicationDbContext _db;

        public TicketAttachmentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<TicketAttachment> CreateAsync(TicketAttachment attachment)
        {
            await _db.TicketAttachments.AddAsync(attachment);
            return attachment;
        }

        public Task DeleteAsync(TicketAttachment attachment)
        {
            _db.TicketAttachments.Remove(attachment);
            return Task.CompletedTask;
        }

        public async Task<TicketAttachment?> GetByIdAsync(int id)
        {
            return await _db.TicketAttachments.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AttachementResponseDTO>> GetByticketIdAsync(int ticketId)
        {
            var attachments = await _db.TicketAttachments.Where(a => a.TicketId == ticketId).Select(a => new AttachementResponseDTO
            {
                Id = a.Id,
                TicketId = a.TicketId,
                FileName = a.FileName,
                FileSize = a.FileSize,
                MimeType = a.MimeType,
                UploadedById = a.UploadedById,
                UploadedByName = a.UploadedBy.FullName,
                UploadedDate = a.UploadedDate
            }).ToListAsync();

            return attachments;
        }
    }
}
