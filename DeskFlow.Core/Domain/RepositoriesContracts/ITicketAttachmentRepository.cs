using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.AttachementDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Domain.RepositoriesContracts
{
    public interface ITicketAttachmentRepository
    {
        Task<TicketAttachment?> GetByIdAsync(int id);
        Task<IEnumerable<AttachementResponseDTO>> GetByticketIdAsync(int ticketId);
        Task<TicketAttachment> CreateAsync(TicketAttachment attachment);
        Task DeleteAsync(TicketAttachment attachment);
    }
}
