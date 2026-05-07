using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DeskFlow.Core.ServicesContracts;
using DeskFlow.Core.DTOs.CategoryDTOs;
using DeskFlow.Core.DTOs.shared;

namespace DeskFlow.WebAPI.Controllers
{
    /// <summary>
    /// Represents an API controller that manages category resources.</summary>
    /// <remarks>Access to this controller is restricted to users with the AdminOnly policy.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(policy: "AdminOnly")]
    
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// initializes a new instance of the <see cref="CategoryController"/> class with the specified category service.
        /// </summary>
        /// <param name="categoryService">The service for managing categories.</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A list of category response DTOs.</returns>
        [ProducesResponseType(typeof(IEnumerable<CategoryResponseDTO>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse),StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDTO category)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetAllCategories), new { id = createdCategory.Id }, createdCategory);
        }

        /// <summary>
        /// Update category by id
        /// </summary>
        /// <param name="id"> Category Id</param>
        /// <param name="category">Category data to update</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDTO category)
        {
            if (id != category.Id)
            {
                return BadRequest("Category ID mismatch.");
            }
            await _categoryService.UpdateAsync(category);
            return NoContent();
        }

        /// <summary>
        /// Delete category by id
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();

        }
    }
}
