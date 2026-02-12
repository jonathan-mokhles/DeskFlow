using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fixi.Core.Domain.Entity
{
    public class TicketStatusHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public TicketStatus FromStatus { get; set; }

        [Required]
        public TicketStatus ToStatus { get; set; }

        [Required]
        public string ChangedById { get; set; }= string.Empty;

        [Required]
        public DateTime ChangedDate { get; set; }

        [MaxLength(500)]
        public string? ChangeReason { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(TicketId))]
        public Ticket Ticket { get; set; } = null!;

        [ForeignKey(nameof(ChangedById))]
        public ApplicationUser ChangedBy { get; set; } = null!;
    }
}
