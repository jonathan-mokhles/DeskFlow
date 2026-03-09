using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Fixi.Core.DTOs.TicketDTOs
{
    public  record TicketAuditHistoryDTO
    {
        public string ChangeType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string ChangedByName { get; set; } = string.Empty;
        public DateTime ChangedDate { get; set; }
        public string? ChangeReason { get; set; }
    }
}
