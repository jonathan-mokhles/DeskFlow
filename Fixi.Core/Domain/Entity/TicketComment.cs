using Fixi.Core.Domain.IdentityEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fixi.Core.Domain.Entity
{
    public class TicketComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string CommentText { get; set; } = string.Empty;

        [Required]
        public bool IsInternal { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(TicketId))]
        public Ticket Ticket { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;
    }
}
