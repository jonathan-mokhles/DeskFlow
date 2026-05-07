using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.DepartmentDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.ServicesContracts
{
    public interface IDepartmentService
    {
        public Task<DepartmentResponseDTO> CreateDepartmentAsync(DepartmentCreateDTO createDTO);
        public Task DeleteDepartmentAsync(int Id);
        public Task<IEnumerable<DepartmentResponseDTO>> GetAllDepartmentsAsync();
    }
}
