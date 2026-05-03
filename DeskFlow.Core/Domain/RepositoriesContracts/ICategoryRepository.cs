using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Domain.RepositoriesContracts
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int Id);

    }
}
