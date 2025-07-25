﻿using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Response;

namespace NestLeaf.Services
{
    public interface IcategoryService
    {
        Task<Category> AddCategory(AddCategoryDto dto);
        Task<bool> UpdateCategory(UpdateCategoryDto dto);
    }
}
