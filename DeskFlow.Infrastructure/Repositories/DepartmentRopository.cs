using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Infrastructure.Repositories
{
    public class DepartmentRopository : IDepartmentRepository
    {
        ApplicationDbContext _db;

        public DepartmentRopository(ApplicationDbContext db)
        {
                _db = db;
        }


        public async Task<Department> CreateAsync(string DepartmentName)
        {
            var department = new Department { Name = DepartmentName };
            await _db.Departments.AddAsync(department);
            return department;
        }

        public Task DeleteAsync(int id)
        {
            _db.Departments.Remove(new Department { Id = id });
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
              return await _db.Departments.ToListAsync();
        }
    }
}
