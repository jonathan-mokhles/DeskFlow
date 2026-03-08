using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.DTOs.TicketDTOs
{
    public class CreateTicketDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string ReportedById { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}
