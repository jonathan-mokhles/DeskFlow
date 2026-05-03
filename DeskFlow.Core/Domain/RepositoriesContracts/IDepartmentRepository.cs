using DeskFlow.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Domain.RepositoriesContracts
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department> CreateAsync(string  DepartmentName);
        Task DeleteAsync(int id);
    }
}
