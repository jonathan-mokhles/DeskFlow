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
        IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            var category = await _unitOfWork.Category.CreateAsync(new Category
            {
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                DepartmentId = categoryDTO.DepartmentId
            });
            await _unitOfWork.CommitAsync();

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
            await _unitOfWork.Category.DeleteAsync(Id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var result = await _unitOfWork.Category.GetAllAsync();

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
            return UpdateAndCommitAsync(categoryDTO);
        }

        private async Task UpdateAndCommitAsync(UpdateCategoryDTO categoryDTO)
        {
            await _unitOfWork.Category.UpdateAsync(new Category
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                DepartmentId = categoryDTO.DepartmentId
            });
            await _unitOfWork.CommitAsync();
        }
    }
}
