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

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>(false, "Invalid data", null));

            var newCategory = await _categoryService.AddCategory(dto);
            return Ok(new ApiResponse<Category>(true, "Category added successfully", newCategory));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new ApiResponse<string>(false, "ID mismatch", null));

            var result = await _categoryService.UpdateCategory(dto);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }


    }
}
