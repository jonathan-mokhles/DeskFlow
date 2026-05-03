using DeskFlow.Core.Domain.IdentityEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeskFlow.Core.Domain.Entity
{
    public class TicketAuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ChangeType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? OldValue { get; set; }

        [MaxLength(500)]
        public string? NewValue { get; set; }

        [Required]
        public string ChangedById { get; set; } = string.Empty;

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