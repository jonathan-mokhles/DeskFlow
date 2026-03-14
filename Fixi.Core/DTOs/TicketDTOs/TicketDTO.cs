using Fixi.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.DTOs.TicketDTOs
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public TicketPriority priority { get; set; }
        public string? AssignedToId { get; set; } = null;
        public string? AssignedToFullname { get; set; }
        public string? ReportedById { get; set; }
        public TicketStatus status { get; set; }
    }
}
