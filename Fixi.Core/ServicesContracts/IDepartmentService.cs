using DeskFkow.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.ServicesContracts
{
    public interface IDepartmentService
    {
        public Task<Department> CreateDepartmentAsync(string name);
        public Task DeleteDepartmentAsync(int Id);
        public Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    }
}
