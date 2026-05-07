using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.DepartmentDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Domain.RepositoriesContracts
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<DepartmentResponseDTO>> GetAllAsync();
        Task<DepartmentResponseDTO> CreateAsync(Department department);
        Task DeleteAsync(int id);
    }
}
