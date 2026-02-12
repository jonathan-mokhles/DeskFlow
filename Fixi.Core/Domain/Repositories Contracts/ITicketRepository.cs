using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(int id);
        Task<Ticket?> GetByTicketNumberAsync(string ticketNumber);
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket> CreateAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);
    }
}
