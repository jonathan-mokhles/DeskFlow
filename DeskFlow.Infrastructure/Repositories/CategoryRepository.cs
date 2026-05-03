using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.DTOs.CategoryDTOs;
using DeskFlow.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Infrastructure.Repositories
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
            return category;
        }

        public Task DeleteAsync(int Id)
        {
            var category = _db.Categories.Find(Id);
            if (category != null)
            {
                _db.Categories.Remove(category);
            }
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _db.Categories.ToListAsync();
        }

        public Task UpdateAsync(Category category)
        {
            _db.Categories.Update(category);
            return Task.CompletedTask;
        }
    }
}
