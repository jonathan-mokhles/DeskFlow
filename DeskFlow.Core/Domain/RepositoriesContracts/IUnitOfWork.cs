using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Domain.RepositoriesContracts
{
    public interface IUnitOfWork : IDisposable
    {
        ITicketRepository Ticket { get; }
        ITicketAuditLogRepository TicketAuditLog { get; }
        ITicketCommentRepository TicketComment { get; }
        ISLASettingRepository SLASetting { get; }
        ICategoryRepository Category { get; }
        IDepartmentRepository Department { get; }
        IUserRepository User { get; }
        ITicketAttachmentRepository TicketAttachment { get; }
        public Task<int> CommitAsync();
    }
}
