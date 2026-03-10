using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fixi.Core.DTOs.TicketDTOs
{
    public record TicketResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CategoryName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; } = string.Empty;
        public string? AssignedToName { get; set; }
        public string? ReportedByName { get; set; } = string.Empty;
        public bool SLAResponseBreached { get; set; } = false;
        public bool SLAResolutionBreached { get; set; } = false;
    }
}
