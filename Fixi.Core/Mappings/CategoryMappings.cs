using Fixi.Core.Domain.Entity;
using Fixi.Core.DTOs.CategoryDTOs;

namespace Fixi.Core.Mappings
{
    public static class CategoryMappings
    {
        public static Category ToEntity(this CreateCategoryDTO dto)
        {
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                DepartmentId = dto.DepartmentId
            };
        }

        public static Category ToEntity(this UpdateCategoryDTO dto)
        {
            return new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                DepartmentId = dto.DepartmentId
            };
        }

        public static CategoryResponseDTO ToResponseDto(this Category category)
        {
            return new CategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                DepartmentId = category.DepartmentId,
                DepartmentName = category.Department?.Name ?? string.Empty
            };
        }
    }
}
