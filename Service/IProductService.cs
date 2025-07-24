using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Response;
namespace NestLeaf.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProduct();

        Task<ProductDto> GetProductById(int id);

        Task<Product>AddProduct(AddProductDto dto);

        Task<bool> UpdateProduct([FromBody] UpdateProductDto dto);

        Task<bool> DeleteProduct(int id);

        Task<List<ProductwithCategoryDto>> SearchProductsByCategory(string categoryName);


    }
}
