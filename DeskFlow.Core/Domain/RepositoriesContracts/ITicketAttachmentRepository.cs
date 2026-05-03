using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.DTOs.AttachementDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Domain.RepositoriesContracts
{
    public interface ITicketAttachmentRepository
    {
        Task<TicketAttachment?> GetByIdAsync(int id);
        Task<IEnumerable<AttachementResponseDTO>> GetByticketIdAsync(int ticketId);
        Task<TicketAttachment> CreateAsync(TicketAttachment attachment);
        Task DeleteAsync(TicketAttachment attachment);
    }
}
