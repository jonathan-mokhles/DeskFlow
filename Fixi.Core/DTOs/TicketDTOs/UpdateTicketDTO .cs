using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.DTOs.TicketDTOs
{
    public class UpdateTicketDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public int CategoryId { get; set; }
    }
}
