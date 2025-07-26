using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Response;
using NestLeaf.Services;

namespace NestLeaf.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;

        }



        [HttpGet]
        
        public async Task<IActionResult> GetAll()
        {
             
            var getall = await _userService.GetAllUser();

            return Ok(new ApiResponse<List<UserAdminDto>>(
                true, "Users Fetched Successfully", getall)
                );

        }


        [HttpGet("{id}")]
       
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetById(id);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>(false, $"User with ID {id} not found", null)
                );
            }

            return Ok(new ApiResponse<UserAdminDto>(true, "User fetched successfully",user)
               );
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var success = await _userService.DeleteById(id);

            if (!success)
                return NotFound(new ApiResponse<string>(false, $"User with ID {id} not found or User already deleted", null));

            return Ok(new ApiResponse<string>(true, "User deleted successfully", null));
        }



        [HttpPost("blockUnBlock/{id}")]
        public async Task<IActionResult> blockUnBlockUser(int id)
        {
            var result = await _userService.BlockUnBlockUser(id);

            if (result == -1)
            {
                return NotFound(new ApiResponse<object>(false, $"User with ID {id} not found", null));
            }

            var message = result == 1 ? "User blocked successfully" : "User unblocked successfully";

            return Ok(new ApiResponse<object>(true, message, null));
        }



     








    }
}
