using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Response;
using System.Data;

namespace NestLeaf.Services
{
    public class ProductService : IProductService
    {
       
        private readonly IDbConnection _dbConnection;
        private readonly NestLeafDbContext _context;

        public ProductService(IDbConnection dbConnection,NestLeafDbContext context)
        {
          
            _dbConnection = dbConnection;
            _context = context;
        }
        public async Task<List<ProductDto>> GetProduct()
        {
            var getallproducts = await _dbConnection.QueryAsync<ProductDto>("AllProducts", CommandType.StoredProcedure);

            return getallproducts.ToList();  
        }

        public async Task<ProductDto> GetProductById(int id)
        {
            var getbyId = await _dbConnection.QueryFirstOrDefaultAsync<ProductDto>("productbyid", new { productId= id }, commandType: CommandType.StoredProcedure);

            return getbyId;
        }

        public async Task<Product>AddProduct(AddProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                IsActive = true


            };

            _context.Products.Add(product);
           await _context.SaveChangesAsync();

         return product;

        }

        public async Task<ApiResponse<string>> UpdateProduct([FromBody]UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);

            if (product == null)
            {
                return new ApiResponse<string>(false, "Product not found",null); 
            }

            if (!string.IsNullOrEmpty(dto.Name))
                product.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Description))
                product.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                product.ImageUrl = dto.ImageUrl;

            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;

            if (dto.Quantity.HasValue)
                product.Quantity = dto.Quantity.Value;
            if (dto.CategoryId.HasValue)
                product.CategoryId = dto.CategoryId.Value;
           
            product.UpdatedAt = DateTime.Now;
      
            await _context.SaveChangesAsync();

            return new ApiResponse<string>(true, "Product updated successfully",null); 
        }

        public async Task<ApiResponse<string>> DeleteProduct(int id)
        {
            var rowsAffected = await _dbConnection.ExecuteAsync("DeleteProductById", new { ProductId = id }, commandType: CommandType.StoredProcedure);

            if (rowsAffected == 0)
                return new ApiResponse<string>(false, "Product not found or already deleted",null);

            return new ApiResponse<string>(true, "Product deleted successfully",null);
        }


        public async Task<List<ProductwithCategoryDto>> SearchProductsByCategory(string categoryName)
        {


            var products = await _dbConnection.QueryAsync<ProductwithCategoryDto>("SearchByCategory", new { CategoryName = categoryName },commandType: CommandType.StoredProcedure);

            return products.ToList();
        }




    }
}
