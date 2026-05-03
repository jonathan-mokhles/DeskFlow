using DeskFlow.Core.DTOs.shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.DTOs.TicketDTOs
{
    public class TicketQueryParams : PaginationParams
    {
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public int? DepartmentId { get; set; }
        public int? categoryId { get; set; }
        public string? ReporterId { get; set; }
        public string? AssignedToId { get; set; }

        //public string? SortBy { get; set; }
        //public bool IsDecs { get; set; }
    }
}
