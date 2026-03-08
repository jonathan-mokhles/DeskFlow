using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Infrastructure.Repositories
{
    public class TicketAuditLogRepository : ITicketAuditLogRepository
    {
        ApplicationDbContext _db;

        public TicketAuditLogRepository(ApplicationDbContext context)
        {
            _db = context;
        }



        public async Task<TicketAuditLog> CreateAsync(TicketAuditLog history)
        {
            await _db.TicketAuditLog.AddAsync(history);
            await _db.SaveChangesAsync();
            return history;
        }

        public Task<IEnumerable<TicketAuditLog>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TicketAuditLog?> GetByidAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TicketAuditLog?> GetByticketIdAsync(int ticketId)
        {
            throw new NotImplementedException();
        }
    }
}
