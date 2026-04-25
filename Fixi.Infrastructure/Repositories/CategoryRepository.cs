using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.CategoryDTOs;
using Fixi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            return category;

        }

        public Task DeleteAsync(int Id)
        {
            _db.Categories.Remove(_db.Categories.Find(Id)!);
            return _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _db.Categories.ToListAsync();
        }

        public Task UpdateAsync(Category category)
        {
            _db.Categories.Update(category);
            return _db.SaveChangesAsync();
        }
    }
}
