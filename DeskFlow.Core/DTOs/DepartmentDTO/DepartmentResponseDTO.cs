using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.DTOs.DepartmentDTO
{
    public class DepartmentResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ManagerId { get; set; }
        public string? ManagerName { get; set; }
    }
}
