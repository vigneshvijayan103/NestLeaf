using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Response;
using NestLeaf.Services;

namespace NestLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class CategoryController : ControllerBase
    {
        private readonly IcategoryService _categoryService;

        public CategoryController(IcategoryService categoryService)
        {
            _categoryService=categoryService;
        }

        [HttpGet]

        public async Task<IActionResult> GetCategory()
        {
            var result = await _categoryService.GetCategory();

            return Ok(new ApiResponse<List<CategoryViewDto>>(true, "All Category Fetched Succesfully", result));
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>(false, "Invalid data", null));

            var newCategory = await _categoryService.AddCategory(dto);
            return Ok(new ApiResponse<Category>(true, "Category added successfully", newCategory));
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateCategory( [FromBody] UpdateCategoryDto dto)
        {
          
            var result = await _categoryService.UpdateCategory(dto);


            if (!result)
                return NotFound(new ApiResponse<string>(false, "Category not found", null));

            return Ok(new ApiResponse<string>(true, "Category updated successfully", null));
        }


    }
}
