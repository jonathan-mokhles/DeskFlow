using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ITicketAuditLogRepository
    {
        public Task<IEnumerable<TicketAuditLog>> GetByticketIdAsync(int ticketId);
        public Task<TicketAuditLog> CreateAsync(TicketAuditLog history);
    }
}
