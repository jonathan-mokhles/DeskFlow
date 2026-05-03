using DeskFkow.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Domain.RepositoriesContracts
{
    public interface ITicketAuditLogRepository
    {
        public Task<IEnumerable<TicketAuditLog>> GetByticketIdAsync(int ticketId);
        public Task<TicketAuditLog> CreateAsync(TicketAuditLog history);
    }
}
