using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DeskFkow.Core.DTOs.CategoryDTOs
{
    public class CategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
