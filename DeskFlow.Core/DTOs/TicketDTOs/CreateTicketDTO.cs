using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DeskFlow.Core.DTOs.TicketDTOs
{
    public class CreateTicketDTO
    {

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Priority { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
