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

        Task<ApiResponse<string>> UpdateProduct(UpdateProductDto dto);

        Task<ApiResponse<string>> DeleteProduct(int id);

        Task<List<ProductwithCategoryDto>> SearchProductsByCategory(string categoryName);


    }
}
