using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.DTOs.TicketDTOs
{
    public class TicketUpdateStatusDTO
    {
        public int NewStatus { get; set; }
        public string? Comment { get; set; }
    }
}
