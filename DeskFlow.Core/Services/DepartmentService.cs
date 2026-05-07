using System;
using System.Collections.Generic;
using System.Text;
using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.ServicesContracts;
using DeskFlow.Core.DTOs.DepartmentDTO;
using DeskFlow.Core.Mappings;

namespace DeskFlow.Core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DepartmentResponseDTO> CreateDepartmentAsync(DepartmentCreateDTO createDTO)
        {
            var department = await _unitOfWork.Department.CreateAsync(createDTO.ToEntity());
            await _unitOfWork.CommitAsync();
            return department;
        }

        public async Task DeleteDepartmentAsync(int Id)
        {
            await _unitOfWork.Department.DeleteAsync(Id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<DepartmentResponseDTO>> GetAllDepartmentsAsync()
        {
            return await _unitOfWork.Department.GetAllAsync();
        }
    }
}
