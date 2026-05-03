using DeskFkow.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Domain.RepositoriesContracts
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department> CreateAsync(string  DepartmentName);
        Task DeleteAsync(int id);
    }
}
