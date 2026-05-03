using DeskFkow.Core.Domain.IdentityEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeskFkow.Core.Domain.Entity
{
    public  class TicketAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string MimeType { get; set; } = string.Empty;

        [Required]
        public string UploadedById { get; set; } = string.Empty;

        [Required]
        public DateTime UploadedDate { get; set; }


        // Navigation Properties
        [ForeignKey(nameof(TicketId))]
        public Ticket Ticket { get; set; } = null!;

        [ForeignKey(nameof(UploadedById))]
        public ApplicationUser UploadedBy { get; set; } = null!;
    }
}

