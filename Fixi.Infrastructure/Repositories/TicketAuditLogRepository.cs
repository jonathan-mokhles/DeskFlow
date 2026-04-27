using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
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
            return history;
        }

        public async Task<IEnumerable<TicketAuditLog>> GetByticketIdAsync(int ticketId)
        {
            return await _db.TicketAuditLog.Where(a => a.TicketId == ticketId).ToListAsync();
        }
    }
}
