using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Models;
using NestLeaf.Response;
using NestLeaf.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace NestLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userservice)
        {
             _userService = userservice;
        }

        [HttpPost("register")] 

        public async Task<IActionResult> Register([FromBody]UserRegisterDto dto)
        {

       

            var user = await _userService.RegisterUser(dto);

            if (user == null)
            
                return Conflict(new ApiResponse<object>(false, "User with same Username or Same Phone number exists", null));

                

            return Ok(new ApiResponse<UserRegisterDto>(true, "User registered", user)

                    );

                  
        }
       


        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userlog =await _userService.UserLogin(loginDto);
         

           if(userlog == null)
            {

                return Unauthorized(new ApiResponse<object>(false, "Invalid username or password.", null));
            }

            if (userlog.Token == "blocked")
            {
                return StatusCode(403, (new ApiResponse<object>(false, "User is blocked,Please Contact Support", null)));
            }



            return Ok(new ApiResponse<LoginResponseDto>(true, "Login successful", userlog));

        }

   
    }

}
