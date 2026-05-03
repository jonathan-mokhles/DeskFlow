using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Infrastructure.Repositories
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
