using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DeskFlow.Core.DTOs.DepartmentDTO
{
    public class DepartmentCreateDTO
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? ManagerId { get; set; }
    }
}
