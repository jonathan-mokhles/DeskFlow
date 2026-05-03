using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.TicketDTOs;
using DeskFlow.Core.Enums;

namespace DeskFlow.Core.Mappings
{
    public static class TicketMappings
    {
        public static Ticket ToEntity(this CreateTicketDTO dto)
        {
            return new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = (TicketPriority)dto.Priority,
                CategoryId = dto.CategoryId
            };
        }

        public static Ticket ToEntity(this UpdateTicketDTO dto)
        {
            return new Ticket
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Priority = (TicketPriority)dto.Priority,
                CategoryId = dto.CategoryId
            };
        }

        public static TicketDTO ToTicketDto(this Ticket ticket)
        {
            return new TicketDTO
            {
                Id = ticket.Id,
                DepartmentId = ticket.Category?.DepartmentId ?? 0,
                priority = ticket.Priority,
                AssignedToId = ticket.AssignedToId,
                AssignedToFullname = ticket.AssignedTo?.FullName,
                ReportedById = ticket.ReportedById,
                status = ticket.Status
            };
        }

        public static TicketResponseDTO ToResponseDto(this Ticket ticket)
        {
            return new TicketResponseDTO
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                CategoryName = ticket.Category?.Name ?? string.Empty,
                DepartmentName = ticket.Category?.Department?.Name ?? string.Empty,
                AssignedToName = ticket.AssignedTo?.FullName,
                ReportedByName = ticket.ReportedBy?.FullName ?? string.Empty,
                SLAResponseBreached = ticket.SLAResponseBreached,
                SLAResolutionBreached = ticket.SLAResolutionBreached
            };
        }

        public static TicketFullResponseDTO ToFullResponseDto(this Ticket ticket)
        {
            return new TicketFullResponseDTO
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedDate = ticket.CreatedDate,
                AssignedDate = ticket.AssignedDate,
                ResolvedDate = ticket.ResolvedDate,
                ClosedDate = ticket.ClosedDate,
                LastModifiedDate = ticket.LastModifiedDate,
                SLAResponseDeadline = ticket.SLAResponseDeadline,
                SLAResponseBreached = ticket.SLAResponseBreached,
                SLAResolutionDeadline = ticket.SLAResolutionDeadline,
                SLAResolutionBreached = ticket.SLAResolutionBreached,
                LastModifiedById = ticket.LastModifiedById,
                CategoryName = ticket.Category?.Name ?? string.Empty,
                DepartmentName = ticket.Category?.Department?.Name ?? string.Empty,
                ReportedByName = ticket.ReportedBy?.FullName ?? string.Empty,
                AssignedToName = ticket.AssignedTo?.FullName,
                LastModifiedByName = ticket.LastModifiedBy?.FullName ?? string.Empty
            };
        }

        public static TicketAuditHistoryDTO ToHistoryDto(this TicketAuditLog auditLog)
        {
            return new TicketAuditHistoryDTO
            {
                ChangeType = auditLog.ChangeType,
                OldValue = auditLog.OldValue,
                NewValue = auditLog.NewValue,
                ChangedByName = auditLog.ChangedBy?.FullName ?? string.Empty,
                ChangedDate = auditLog.ChangedDate,
                ChangeReason = auditLog.ChangeReason
            };
        }
    }
}
