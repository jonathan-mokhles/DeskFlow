using System;
using System.Collections.Generic;
using System.Text;
using DeskFlow.Core.DTOs.DepartmentDTO;
using DeskFlow.Core.Domain.Entity;

namespace DeskFlow.Core.Mappings
{
    public static class DepartmentMappings
    {
        public static IEnumerable<DepartmentResponseDTO> ToResponseDTOs(this IEnumerable<Department> departments)
        {
            var departmentDTOs = new List<DepartmentResponseDTO>();
            foreach (var department in departments)
            {
                departmentDTOs.Add(new DepartmentResponseDTO
                {
                    Id = department.Id,
                    Name = department.Name,
                    ManagerId = department.ManagerId,
                    ManagerName = department.Manager?.FullName
                });
            }
            return departmentDTOs;
        }

        public static Department ToEntity(this DepartmentCreateDTO departmentCreateDTO)
        {
            return new Department
            {
                Name = departmentCreateDTO.Name,
                ManagerId = departmentCreateDTO.ManagerId
            };
        }
    }
}
