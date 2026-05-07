using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.DTOs.DepartmentDTO;
using DeskFlow.Core.Exceptions;
using DeskFlow.Core.Mappings;
using DeskFlow.Infrastructure.DbContext;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Infrastructure.Repositories
{
    public class DepartmentRopository : IDepartmentRepository
    {
        ApplicationDbContext _db;

        public DepartmentRopository(ApplicationDbContext db)
        {
                _db = db;
        }


        public async Task<DepartmentResponseDTO> CreateAsync(Department department)
        {
            var Manager = _db.Users.FirstOrDefault(u => u.Id == department.ManagerId);
            if (Manager == null)
            {
                throw new NotFoundException("Manager not found");
            }
            await _db.Departments.AddAsync(department);
            await _db.SaveChangesAsync();
            return new DepartmentResponseDTO
            {
                Id = department.Id,
                Name = department.Name,
                ManagerId = department.ManagerId,
                ManagerName = Manager.FullName
            };
        }

        public Task DeleteAsync(int id)
        {
            _db.Departments.Remove(new Department { Id = id });
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<DepartmentResponseDTO>> GetAllAsync()
        {
             var departments = await _db.Departments.Include(d => d.Manager).ToListAsync();
             return departments.ToResponseDTOs();
        }
    }
}
