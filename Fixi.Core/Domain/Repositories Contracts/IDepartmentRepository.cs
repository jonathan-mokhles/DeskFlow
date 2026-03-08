using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ticketIdepartmentRepository
    {
        // Basic CRUD
        Task<Department?> GetByticketIdAsync(int ticketId);
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department> CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int ticketId);
    }
}
