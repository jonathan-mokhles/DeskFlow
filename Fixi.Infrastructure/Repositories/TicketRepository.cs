using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Fixi.Core.Enums;

namespace Fixi.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        ApplicationDbContext _db;

        public TicketRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            await _db.Tickets.AddAsync(ticket);
            await _db.SaveChangesAsync();
            return ticket;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllAsync(TicketQueryParams queryParams)
        {
            var query = _db.Tickets.AsQueryable();
            // Filtering
            if (queryParams.Status is not null)
            {
                query = query.Where(t => (int)t.Status == queryParams.Status);
            }
            if (queryParams.Priority is not null)
            {
                query = query.Where(t => (int)t.Priority == queryParams.Priority);
            }
            if (!string.IsNullOrEmpty(queryParams.AssignedToId))
            {
                query = query.Where(t => t.AssignedToId != null && t.AssignedToId == queryParams.AssignedToId);
            }
            if (!string.IsNullOrEmpty(queryParams.ReporterId))
            {
                query = query.Where(t => t.ReportedById == queryParams.ReporterId);
            }
            if (queryParams.DepartmentId is not null)
            {
                query = query.Where(t => t.Category.DepartmentId == queryParams.DepartmentId);
            }
            if (queryParams.categoryId is not null)
            {
                query = query.Where(t => t.CategoryId == queryParams.categoryId);
            }

            queryParams.PageSize = Math.Min(queryParams.PageSize, 100);
            int skip = (queryParams.PageNumber - 1) * queryParams.PageSize;

            return await query.Select(t => new TicketResponseDTO
            {
                Id = t.Id,
                Title = t.Title,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                AssignedToName = t.AssignedTo != null ? t.AssignedTo.FullName : null,
                ReportedByName = t.ReportedBy != null ? t.ReportedBy.FullName : null,
                SLAResponseBreached = t.SLAResponseBreached,
                SLAResolutionBreached = t.SLAResolutionBreached,
                CategoryName = t.Category != null ? t.Category.Name : null,
                DepartmentName = t.Category != null && t.Category.Department != null ? t.Category.Department.Name! : null
            }).Skip(skip).Take(queryParams.PageSize).ToListAsync();
        }

        public async Task UpdateAsync(UpdateTicketDTO ticket)
        {
            await _db.Tickets.Where(t => t.Id == ticket.Id).ExecuteUpdateAsync(t => t
                .SetProperty(t => t.Title, ticket.Title)
                .SetProperty(t => t.Description, ticket.Description)
                .SetProperty(t => t.Priority, (TicketPriority)ticket.Priority)
                .SetProperty(t => t.CategoryId, ticket.CategoryId)
                .SetProperty(t => t.LastModifiedDate, DateTime.UtcNow)
                .SetProperty(t => t.LastModifiedById, t => t.ReportedById)
                );
        }

        public async Task DeleteAsync(int id)
        {
            _db.Tickets.Remove(new Ticket { Id = id });
            await _db.SaveChangesAsync();
        }

        public async Task<TicketFullResponseDTO?> GetFullTicketAsync(int ticketId)
        {
            return await _db.Tickets.Where(t => t.Id == ticketId).Select(t => new TicketFullResponseDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                CreatedDate = t.CreatedDate,
                AssignedDate = t.AssignedDate,
                ResolvedDate = t.ResolvedDate,
                ClosedDate = t.ClosedDate,
                LastModifiedDate = t.LastModifiedDate,
                SLAResponseDeadline = t.SLAResponseDeadline,
                SLAResponseBreached = t.SLAResponseBreached,
                SLAResolutionDeadline = t.SLAResolutionDeadline,
                SLAResolutionBreached = t.SLAResolutionBreached,
                LastModifiedById = t.LastModifiedById,
                CategoryName = t.Category.Name,
                DepartmentName = t.Category.Department.Name!,
                ReportedByName = t.ReportedBy.FullName,
                AssignedToName = t.AssignedTo != null ? t.AssignedTo.FullName : null,
                LastModifiedByName = t.LastModifiedBy.FullName
            }).FirstOrDefaultAsync();

        }

        public async Task UpdateStatus(int ticketId, TicketStatus newStatus)
        {
            await _db.Tickets.Where(t => t.Id == ticketId).ExecuteUpdateAsync(t => t.SetProperty(t => t.Status, newStatus));
        }

        public async Task UpdatePriority(int ticketId, int newPriority)
        {
            await _db.Tickets.Where(t => t.Id == ticketId).ExecuteUpdateAsync(t => t.SetProperty(t => t.Priority, (TicketPriority)newPriority));
        }

        public async Task AssignTechnician(int ticketId, string technicianId)
        {
            await _db.Tickets.Where(t => t.Id == ticketId).ExecuteUpdateAsync(t => t.SetProperty(t => t.AssignedToId, technicianId));
        }

        public async Task<TicketDTO?> GetTicketAsync(int ticketId)
        {
            return await _db.Tickets
                .Where(t => t.Id == ticketId)
                .Select(t => new TicketDTO
                {
                    DepartmentId = t.Category.DepartmentId,
                    priority = t.Priority,
                    status = t.Status,
                    ReportedById = t.ReportedById,
                    AssignedToId = t.AssignedToId,
                    AssignedToFullname = t.AssignedTo != null ? t.AssignedTo.FullName : null
                })
                .FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<TicketAuditHistoryDTO>> GetTicketHisoryAsync(int ticketId)
        {
            return await _db.TicketAuditLog.Where(a => a.TicketId == ticketId).Select(a => new TicketAuditHistoryDTO
            {
                ChangeType = a.ChangeType,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                ChangedByName = a.ChangedBy.FullName,
                ChangedDate = a.ChangedDate,
                ChangeReason = a.ChangeReason
            }).ToListAsync();
        }
    }
}
