using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Fixi.Core.Enums;



namespace Fixi.Core.DTOs.SLADTOs
{
    public  class SLACreateDTO
    {

        [Required]
        public TicketPriority Priority { get; set; }

        [Required]
        public int ResponseTimeMinutes { get; set; }

        [Required]
        public int ResolutionTimeMinutes { get; set; }
    }
}
