using Fixi.Core.Domain.Entity;
using Fixi.Core.DTOs.AttachementDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ITicketAttachmentRepository
    {
        Task<TicketAttachment?> GetByIdAsync(int id);
        Task<IEnumerable<AttachementResponseDTO>> GetByticketIdAsync(int ticketId);
        Task<TicketAttachment> CreateAsync(TicketAttachment attachment);
        Task DeleteAsync(TicketAttachment attachment);
    }
}
