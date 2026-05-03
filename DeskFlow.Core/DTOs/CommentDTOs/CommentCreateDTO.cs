using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DeskFlow.Core.DTOs.CommentDTOs
{
    public class CommentCreateDTO
    {
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
    }
}
