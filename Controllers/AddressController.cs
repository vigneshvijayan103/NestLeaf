using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Response;
using NestLeaf.Service;
using System.Net.Sockets;
using System.Security.Claims;

namespace NestLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

        [HttpPost]
        [Authorize(Roles = "user")]

        public async Task<IActionResult> AddAddress([FromBody] AddressDto dto)
        {
           
            var userId=GetUserId();

            var result = await _addressService.Addaddress(dto, userId);
            if (!result)
                return StatusCode(500, new ApiResponse<string>(false, "Failed to add address", null));


            return Ok(new ApiResponse<string>(true, "Address added successfully", null));
        }



        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = GetUserId();
            var data = await _addressService.GetAddresses(userId);

            return Ok(new ApiResponse<List<AddressDto>>(true, "Addresses fetched", data));
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDto dto)
        {
           


            var userId = GetUserId();
            var success = await _addressService.UpdateAddress(id,dto,  userId);
            if (!success)
                return NotFound(new ApiResponse<string>(false, "Address not found", null));

            return Ok(new ApiResponse<string>(true, "Address updated successfully", null));
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = GetUserId();
            var success = await _addressService.DeleteAddress(id, userId);
            if (!success)
                return NotFound(new ApiResponse<string>(false, "Address not found", null));

            return Ok(new ApiResponse<string>(true, "Address deleted successfully", null));
        }
    }
}

