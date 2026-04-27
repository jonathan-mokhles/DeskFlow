using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Infrastructure.DbContext;

namespace Fixi.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ITicketRepository Ticket { get; }
        public ITicketAuditLogRepository TicketAuditLog { get; }
        public ITicketCommentRepository TicketComment { get; }
        public ISLASettingRepository SLASetting { get; }
        public ICategoryRepository Category { get; }
        public IDepartmentRepository Department { get; }
        public IUserRepository User { get; }
        public ITicketAttachmentRepository TicketAttachment { get; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Ticket = new TicketRepository(_context);
            TicketAuditLog = new TicketAuditLogRepository(_context);
            TicketComment = new TicketCommentRepository(_context);
            SLASetting = new SLASettingRepository(_context);
            Category = new CategoryRepository(_context);
            Department = new DepartmentRopository(_context);
            User = new UserRepository(_context);
            TicketAttachment = new TicketAttachmentRepository(_context);
        }


        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}