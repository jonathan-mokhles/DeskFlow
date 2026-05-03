using DeskFkow.Core.Domain.IdentityEntity;
using DeskFkow.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeskFkow.Core.Domain.Entity
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TicketPriority Priority { get; set; }

        [Required]
        public TicketStatus Status { get; set; }

        // References
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string ReportedById { get; set; } = string.Empty;
        public string? AssignedToId { get; set; }

        // Dates
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }


        // SLA 
        [Required]
        public DateTime SLAResponseDeadline { get; set; }
        public bool SLAResponseBreached { get; set; } = false;

        [Required]
        public DateTime SLAResolutionDeadline { get; set; }
        public bool SLAResolutionBreached { get; set; } = false;


        // Audit
        [Required]
        public string LastModifiedById { get; set; } = string.Empty;



        // Navigation Properties
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        [ForeignKey(nameof(ReportedById))]
        public ApplicationUser ReportedBy { get; set; } = null!;

        [ForeignKey(nameof(AssignedToId))]
        public ApplicationUser? AssignedTo { get; set; }

        [ForeignKey(nameof(LastModifiedById))]
        public ApplicationUser LastModifiedBy { get; set; } = null!;

        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
        public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
        public ICollection<TicketAuditLog> AuditLogs { get; set; } = new List<TicketAuditLog>();
    }
}
