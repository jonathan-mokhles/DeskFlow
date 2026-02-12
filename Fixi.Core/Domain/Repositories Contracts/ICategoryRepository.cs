using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ICategoryRepository
    {
        // Basic CRUD
        Task<Category?> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);

    }
}
