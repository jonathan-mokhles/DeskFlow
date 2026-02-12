using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ITicketAttachmentRepository
    {
        // Basic CRUD
        Task<TicketAttachment?> GetByIdAsync(int id);
        Task<IEnumerable<TicketAttachment>> GetAllAsync();
        Task<TicketAttachment> CreateAsync(TicketAttachment attachment);
        Task UpdateAsync(TicketAttachment attachment);
        Task DeleteAsync(int id);
    }
}
