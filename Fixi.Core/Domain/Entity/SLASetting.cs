using DeskFkow.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeskFkow.Core.Domain.Entity
{
    public class SLASetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TicketPriority Priority { get; set; }

        [Required]
        public int ResponseTimeMinutes { get; set; }

        [Required]
        public int ResolutionTimeMinutes { get; set; }

    }
}
