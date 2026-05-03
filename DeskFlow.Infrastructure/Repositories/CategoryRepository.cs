using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.CategoryDTOs;
using DeskFkow.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Infrastructure.Repositories
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
            _db.Categories.Remove(_db.Categories.Find(Id)!);
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
