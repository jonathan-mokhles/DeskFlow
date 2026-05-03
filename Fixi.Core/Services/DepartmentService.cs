using System;
using System.Collections.Generic;
using System.Text;
using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.ServicesContracts;

namespace DeskFkow.Core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Department> CreateDepartmentAsync(string name)
        {
            var department = await _unitOfWork.Department.CreateAsync(name);
            await _unitOfWork.CommitAsync();
            return department;
        }

        public async Task DeleteDepartmentAsync(int Id)
        {
            await _unitOfWork.Department.DeleteAsync(Id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _unitOfWork.Department.GetAllAsync();
        }
    }
}
