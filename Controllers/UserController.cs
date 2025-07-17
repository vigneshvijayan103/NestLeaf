using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Services;
using NestLeaf.Response;

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



        [HttpPost("{id}/block")]
        public async Task<IActionResult>BlockUser(int id)
        {
            var isBlock = await _userService.BlockUser(id);

            if (!isBlock)
            {
                return NotFound(new ApiResponse<object>(false, $"User with ID {id} not found", null
                    ));
            }

            return Ok(new ApiResponse<object>(true, "User Blocked successfully", null)
                );
        }



        [HttpPost("{id}/unblock")]
        public async Task <IActionResult> UnblockUser(int id)
        {
            var isUnBlock = await _userService.Unblock(id);

            if (!isUnBlock)
            {
                return NotFound(new ApiResponse<object>(false, $"User with ID {id} not found", null
                    ));
            }


            return Ok(new ApiResponse<object>(true, "User Unblock successfully", null)
                );

        }









    }
}
