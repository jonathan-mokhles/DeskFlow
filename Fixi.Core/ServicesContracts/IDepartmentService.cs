using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public interface IDepartmentService
    {
        public Task CreateDepartmentAsync(string name);
        public Task DeleteDepartmentAsync(int Id);
        public Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    }
}
