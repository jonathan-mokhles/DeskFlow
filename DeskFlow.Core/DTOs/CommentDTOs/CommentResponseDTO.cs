using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DeskFkow.Core.DTOs.CommentDTOs
{
    public class CommentResponseDTO
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string CommentText { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
