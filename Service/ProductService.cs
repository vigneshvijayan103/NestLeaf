using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public async Task<List<ProductDto>> SearchProductsByNameAsync(string name)
        {
          
            var parameters = new { Name = name };

            var result = await _dbConnection.QueryAsync<ProductDto>(
                "SearchProductsByName",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }



        public async Task<bool> UpdateProduct([FromBody]UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);

            if (product == null)
            {
                   return false;
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

          

            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var rowsAffected = await _dbConnection.ExecuteAsync("DeleteProductById", new { ProductId = id }, commandType: CommandType.StoredProcedure);

            if (rowsAffected == 0)
                return false;
               
            return true;
        }

        public async Task<bool> ToggleProductActiveStatus(int productId, bool isActive)
        {
            var rowsAffected = await _dbConnection.ExecuteAsync(
                "ToggleProductActiveStatus",
                new { ProductId = productId, IsActive = isActive },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }



        public async Task<List<ProductwithCategoryDto>> SearchProductsByCategory(string categoryName)
        {


            var products = await _dbConnection.QueryAsync<ProductwithCategoryDto>("SearchByCategory", new { CategoryName = categoryName },commandType: CommandType.StoredProcedure);

            return products.ToList();
        }

        public async Task<PaginatedResult<ProductDto>> GetPaginatedFilteredProducts(ProductFilterDto dto)
        {
            var result = new PaginatedResult<ProductDto>();

            var parameters = new
            {
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                CategoryId = dto.CategoryId,
                MinPrice = dto.MinPrice,
                MaxPrice = dto.MaxPrice
               
            };
                using var multi = await _dbConnection.QueryMultipleAsync(
              "FilterPaginatedProducts",
                 parameters,
               commandType: CommandType.StoredProcedure
            );


            result.Items = (await multi.ReadAsync<ProductDto>()).ToList();
            result.TotalCount = await multi.ReadFirstAsync<int>();

            return result;
        }








    }
}
