using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Infrastructure.Repositories
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
