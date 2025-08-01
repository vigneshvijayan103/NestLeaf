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

        Task<List<ProductDto>> SearchProductsByNameAsync(string name);

        Task<Product>AddProduct(AddProductDto dto);

        Task<bool> UpdateProduct([FromBody] UpdateProductDto dto);

        Task<bool> DeleteProduct(int id);

        Task<bool> ToggleProductActiveStatus(int productId, bool isActive);

        Task<List<ProductwithCategoryDto>> SearchProductsByCategory(string categoryName);

      
      Task<PaginatedResult<ProductDto>> GetPaginatedFilteredProducts(ProductFilterDto dto);


    }
}
