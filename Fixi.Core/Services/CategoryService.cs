using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.CategoryDTOs;
using Fixi.Core.Mappings;
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
            var category = await _unitOfWork.Category.CreateAsync(categoryDTO.ToEntity());
            await _unitOfWork.CommitAsync();

            return category.ToResponseDto();
        }

        public async Task DeleteCategoryAsync(int Id)
        {
            await _unitOfWork.Category.DeleteAsync(Id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var result = await _unitOfWork.Category.GetAllAsync();

            return result.Select(c => c.ToResponseDto());
        }

        public Task UpdateAsync(UpdateCategoryDTO categoryDTO)
        {
            return UpdateAndCommitAsync(categoryDTO);
        }

        private async Task UpdateAndCommitAsync(UpdateCategoryDTO categoryDTO)
        {
            await _unitOfWork.Category.UpdateAsync(categoryDTO.ToEntity());
            await _unitOfWork.CommitAsync();
        }
    }
}
