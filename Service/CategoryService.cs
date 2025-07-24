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

        public async Task<bool> UpdateCategory(UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(dto.Id);
            if (category == null)
                return false;

            category.CategoryName = dto.CategoryName;
            await _context.SaveChangesAsync();

            return true;
        }


    }

}


