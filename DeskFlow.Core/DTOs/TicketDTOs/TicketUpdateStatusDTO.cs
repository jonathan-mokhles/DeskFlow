using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.DTOs.TicketDTOs
{
    public class TicketUpdateStatusDTO
    {
        public int NewStatus { get; set; }
        public string? Comment { get; set; }
    }
}
