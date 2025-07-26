using Microsoft.EntityFrameworkCore;
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

        public async Task <List<CategoryViewDto>> GetCategory()
        {
            var Getcategory = await _context.Categories.Where(e => e.IsDeleted == false)
                                    .Select(c => new CategoryViewDto
                                    {
                                        Id = c.Id,
                                        CategoryName = c.CategoryName,
                                        Description = c.Description
                                    }).ToListAsync();

            return Getcategory;
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


            if (!string.IsNullOrEmpty(dto.CategoryName))
                category.CategoryName = dto.CategoryName;

            if (!string.IsNullOrEmpty(dto.Description))
                category.Description = dto.Description;

                if (dto.IsDeleted.HasValue)
            category.IsDeleted = dto.IsDeleted.Value;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }


   


    }

}


