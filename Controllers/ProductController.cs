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


        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new ApiResponse<string>(false, "Search term cannot be empty.", null));

            var products = await _productService.SearchProductsByNameAsync(name);

            if (products == null || products.Count == 0)
                return NotFound(new ApiResponse<string>(false, "No products found.", null));

            return Ok(new ApiResponse<List<ProductDto>>(true, "Products found.", products));
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

            if (!response)
              
                return NotFound (new ApiResponse<string>(false, "Product not found",null));

            return Ok(response);

            return Ok( new ApiResponse<string>(true, "Product updated successfully",null));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);

            if (!result)
             return NotFound( new ApiResponse<string>(false, "Product not found or already deleted", null));

           
            return Ok( new ApiResponse<string>(true, "Product deleted successfully",null));
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> SetProductActiveInactive(int id, [FromBody] bool isActive)
        {
            var result = await _productService.ToggleProductActiveStatus(id, isActive);

            if (!result)
                return NotFound(new ApiResponse<string>(false, "Product not found or product deleted", null));

            var status = isActive ? "activated" : "deactivated";
            return Ok(new ApiResponse<string>(true, $"Product successfully {status}", null));
        }



        [HttpPost("paginated")]
        public async Task<IActionResult> GetPaginatedProducts([FromBody] ProductFilterDto dto)
        {
            if (dto.PageNumber < 1 || dto.PageSize < 1)
            {
                return BadRequest(new ApiResponse<string>(false, "PageNumber and PageSize must be greater than 0", null));
            }

            var data = await _productService.GetPaginatedFilteredProducts(dto);

            return Ok(new ApiResponse<PaginatedResult<ProductDto>>(true, "Products fetched successfully", data));
        }









    }
}
