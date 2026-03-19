using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Infrastructure.Repositories
{
    public class DepartmentRopository : IDepartmentRepository
    {
        ApplicationDbContext _db;

        public DepartmentRopository(ApplicationDbContext db)
        {
                _db = db;
        }


        public async Task CreateAsync(string DepartmentName)
        {
            await _db.Departments.AddAsync(new Department { Name = DepartmentName });
            await _db.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            _db.Departments.Remove(new Department { Id = id });
            return _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
              return await _db.Departments.ToListAsync();
        }
    }
}
