using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface IDepartmentRepository
    {
        // Basic CRUD
        Task<Department?> GetByIdAsync(int id);
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department> CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
    }
}
