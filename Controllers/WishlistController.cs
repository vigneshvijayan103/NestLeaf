using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Response;
using NestLeaf.Service;
using System.Security.Claims;

namespace NestLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user")]
    public class WishlistController : ControllerBase
    {
        private readonly IwishlistService _iwishlistService;

        public WishlistController(IwishlistService wishlistService)
        {
            _iwishlistService= wishlistService;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }


        [HttpPost]
        public async Task <IActionResult> AddtoWishlist(int productId)
        {
            int userId = GetUserId();
            var result=await _iwishlistService.AddtoWishlist(userId, productId);

            if (result == false)
                return Ok(new ApiResponse<string>(true, "Item already added in whishlist",null));

            return Ok(new ApiResponse<string>(true, "Item  added to whishlist", null));
        }


        [HttpGet]

        public async Task <IActionResult>GetWishList()
        {
            int userId = GetUserId();

            var result=await _iwishlistService.GetWishList(userId);

            if (result == null || !result.Any())
                return Ok(new ApiResponse<string>(true, "Wishlist is empty", null));

            return Ok(new ApiResponse<List<wishlistDto>>(true, "Wishlist fetched successfully", result));
        }

        [HttpDelete]

        public async Task<IActionResult>RemoveFromWishlist( int productId)
        {
            int userId = GetUserId();


            var success =await _iwishlistService.RemoveFromWishlist(userId,productId);

            if (!success)
                return NotFound(new ApiResponse<string>(false, "Item not found in wishList", null));

            return Ok(new ApiResponse<string>(true, "Item removed Successfully", null));

        }

    }
}
