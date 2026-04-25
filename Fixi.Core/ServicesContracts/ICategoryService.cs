using Fixi.Core.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public interface ICategoryService
    {
       public Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO);
       public Task DeleteCategoryAsync(int Id);
       public Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync();
        public Task UpdateAsync(UpdateCategoryDTO categoryDTO);

    }
}
