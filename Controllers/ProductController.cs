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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        public async Task<IActionResult> Getallproducts()
        {
            var products = await _productService.GetProduct();

            return Ok(new ApiResponse<List<ProductDto>>(true, "Fetched all products", products));
        }



        [HttpGet("{id}")]


        public async Task<IActionResult>GetProductsById(int id)
        {
            var getProduct = await _productService.GetProductById(id);

            if(getProduct==null) 
                return NotFound(new ApiResponse<string>(false, "Wrong product Id", null));

            return Ok(new ApiResponse<ProductDto>(true, "Fetched  product successfully", getProduct));
        }

        [HttpGet("search-by-category")]
        public async Task<IActionResult> SearchByCategory([FromQuery] string categoryName)
        {
            var products = await _productService.SearchProductsByCategory(categoryName);

            if (products == null || !products.Any())
            {
                return NotFound(new ApiResponse<List<ProductwithCategoryDto>>(
                    false,
                    $"No products found for category '{categoryName}'",
                    null
                ));
            }

            return Ok(new ApiResponse<List<ProductwithCategoryDto>>(true, "Products found", products));
        }


        [Authorize(Roles = "admin")]
        [HttpPost]

        public async Task<IActionResult> AddProduct([FromBody]AddProductDto dto)
        {
            var newProduct=await _productService.AddProduct(dto);

            return Ok(new ApiResponse<Product>(true, "Product added successfully", newProduct));
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("update")]

        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var response = await _productService.UpdateProduct(dto);

            if (!response.Success)
                return NotFound(response); 

            return Ok(response);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

      







    }
}
