using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.IdentityEntity;
using DeskFkow.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeskFkow.Core.DTOs.TicketDTOs
{
    public record TicketFullResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }

        // Dates
        public DateTime CreatedDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        [Required]
        public DateTime SLAResponseDeadline { get; set; }
        public bool SLAResponseBreached { get; set; } = false;

        [Required]
        public DateTime SLAResolutionDeadline { get; set; }
        public bool SLAResolutionBreached { get; set; } = false;

        public string LastModifiedById { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        public string ReportedByName { get; set; } = string.Empty;
        public string? AssignedToName { get; set; } = string.Empty;
        public string LastModifiedByName { get; set; } = string.Empty;
    }
}
