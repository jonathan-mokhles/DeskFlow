using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ITicketCommentRepository
    {
        // Basic CRUD
        Task<TicketComment?> GetByticketIdAsync(int ticketId);
        Task<IEnumerable<TicketComment>> GetAllAsync();
        Task<TicketComment> CreateAsync(TicketComment comment);
        Task UpdateAsync(TicketComment comment);
        Task DeleteAsync(int ticketId);
    }
}
