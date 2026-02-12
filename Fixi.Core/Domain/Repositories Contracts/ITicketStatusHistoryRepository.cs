using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ITicketStatusHistoryRepository
    {
        // Basic CRUD
        Task<TicketStatusHistory?> GetByIdAsync(int id);
        Task<IEnumerable<TicketStatusHistory>> GetAllAsync();
        Task<TicketStatusHistory> CreateAsync(TicketStatusHistory history);
    }
}
