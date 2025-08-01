using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Enum;
using NestLeaf.Models;
using NestLeaf.Response;
using NestLeaf.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NestLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IadminService _iadminService;

        public AdminDashboardController(IadminService iadminService)
        {
            _iadminService = iadminService;
        }

        [HttpGet("Total-Revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var revenue=await _iadminService.GetTotalRevenue();

            return Ok(new ApiResponse<decimal>(true, "Revenue fetched successfully",revenue));
        }

        [HttpGet("Revenue-by-date")]
        public async Task<IActionResult> GetRevenueByDate(DateTime startDate, DateTime endDate)
        {
            var revenue=await _iadminService.GetRevenueByDateAsync(startDate, endDate);

            return Ok(new ApiResponse<decimal>(true, "Revenue fetched successfully", revenue));
        }

        [HttpGet("TotalUsers")]
        public async Task< IActionResult>TotalUsers()
        {
            var users = await _iadminService.TotalUsers();

            return Ok(new ApiResponse<int>(true, "Total user fetched successfully",users));

        }

    }
}
