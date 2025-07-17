using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Response;
using System;

namespace NestLeaf.Services
{
    public class CategoryService:IcategoryService

    {
        private readonly NestLeafDbContext _context;

        public CategoryService(NestLeafDbContext context)
        {
            _context = context;
        }
        public async Task<Category> AddCategory(AddCategoryDto dto)
        {

            var category = new Category
            {

                CategoryName = dto.CategoryName
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
             
            return category;
        }

        public async Task<ApiResponse<string>> UpdateCategory(UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(dto.Id);

            if (category == null)
                return new ApiResponse<string>(false, "Category not found", null);

            category.CategoryName = dto.CategoryName;
            await _context.SaveChangesAsync();

            return new ApiResponse<string>(true, "Category updated successfully", null);
        }


    }

}


