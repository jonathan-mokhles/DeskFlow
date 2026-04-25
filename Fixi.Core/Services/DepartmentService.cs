using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.ServicesContracts;

namespace Fixi.Core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<Department> CreateDepartmentAsync(string name)
        {
            return await _departmentRepository.CreateAsync(name);
        }

        public async Task DeleteDepartmentAsync(int Id)
        {
            await _departmentRepository.DeleteAsync(Id);
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }
    }
}
