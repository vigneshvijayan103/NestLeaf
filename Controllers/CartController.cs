
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Response;
using NestLeaf.Service;
using System.Security.Claims;

﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace NestLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize(Roles = "user")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

        [HttpPost]

        public async Task<IActionResult> AddtoCart([FromBody] AddCartDto dto)

        {
            int userId = GetUserId();

            try
            {
                var result = await _cartService.AddtoCart(dto, userId);

                return Ok(new ApiResponse<CartItemDto>(true, "Product added to cart", result));

            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<string>(false, ex.Message, null));
            }
        }

           


       
        [HttpGet]

        public async Task<IActionResult> GetCart()
        {
            int userId = GetUserId();


            var result = await _cartService.GetCart(userId);

            if (result == null)
            {
                return Ok(new ApiResponse<ViewCartDto>(true, "Cart is empty", result));
            }

            return Ok(new ApiResponse<ViewCartDto>(true, "cart fetched successfully", result));
        }

        [HttpPost("update-quantity")]

        public async Task<IActionResult> UpdateQuantity([FromBody] AddCartDto dto)
        {
            int userId = GetUserId();

            var result = await _cartService.UpdateCartQuantity(dto, userId);

            if (result == null)
                return NotFound(new ApiResponse<ViewCartDto>(false, "Item not found or quantity exceeds", result));

            return Ok(new ApiResponse<ViewCartDto>(true, "Product quantity updated successfully", result));

        }

        [HttpDelete("removeItem/{productId}")]

        public async Task<IActionResult> RemovecartItem(int productId)
        {
            int userId = GetUserId();

            var result = await _cartService.RemoveItem(userId, productId);

            if (result == null)
            {
                return NotFound(new ApiResponse<ViewCartDto>(false, "Item not found in cart", result));
            }

            return Ok(new ApiResponse<ViewCartDto>(true, "Cart Updated Successfully", result));


        }

        [HttpDelete("clearcart")]

        public async Task<IActionResult> DeleteCart()
        {
            int userId = GetUserId();

            var result = await _cartService.DeleteCart(userId);
            if (result == false)
            {
                return NotFound(new ApiResponse<string>(false, "Cart is empty", null));

            }

            return Ok(new ApiResponse<ViewCartDto>(true, "Cart cleared Successfully", null));
        }




    }
}

