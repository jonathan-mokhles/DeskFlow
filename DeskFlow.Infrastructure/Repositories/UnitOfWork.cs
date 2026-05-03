
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Infrastructure.DbContext;

namespace DeskFlow.Infrastructure.Repositories
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
        public UnitOfWork(
            ApplicationDbContext context,
            ITicketRepository ticket,
            ITicketAuditLogRepository ticketAuditLog,
            ITicketCommentRepository ticketComment,
            ISLASettingRepository slaSetting,
            ICategoryRepository category,
            IDepartmentRepository department,
            IUserRepository user,
            ITicketAttachmentRepository ticketAttachment)
        {
            _context = context;

            Ticket = ticket;
            TicketAuditLog = ticketAuditLog;
            TicketComment = ticketComment;
            SLASetting = slaSetting;
            Category = category;
            Department = department;
            User = user;
            TicketAttachment = ticketAttachment;
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