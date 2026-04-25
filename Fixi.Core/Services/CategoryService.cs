using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.CategoryDTOs;
using Fixi.Core.ServicesContracts;

namespace Fixi.Core.Services
{
    public class CategoryService : ICategoryService
    {
        ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }
        public async Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            var category = await _repo.CreateAsync(new Category
            {
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                DepartmentId = categoryDTO.DepartmentId
            });

            return new CategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                DepartmentId = category.DepartmentId,
                DepartmentName = category.Department.Name
            };
        }

        public async Task DeleteCategoryAsync(int Id)
        {
            await _repo.DeleteAsync(Id);
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var result = await _repo.GetAllAsync();

            return result.Select(c => new CategoryResponseDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                DepartmentId = c.DepartmentId,
                DepartmentName = c.Department.Name
            });
        }

        public Task UpdateAsync(UpdateCategoryDTO categoryDTO)
        {
            return  _repo.UpdateAsync(new Category
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                DepartmentId = categoryDTO.DepartmentId
            });
        }
    }
}
