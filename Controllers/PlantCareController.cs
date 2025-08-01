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
    public class PlantCareController : ControllerBase
    {
        private readonly IPlantCareService _plantCareService;

        public PlantCareController(IPlantCareService plantCareService)
        {
            _plantCareService = plantCareService;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }


        [HttpPost("report")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> ReportIssue([FromBody] AssignIssueDto dto)
        {
            var userId = GetUserId();
            var success = await _plantCareService.AddIssueAsync(dto, userId);

            if (!success)
                return BadRequest(new ApiResponse<string>(false, "Failed to report plant issue.", null));

            return Ok(new ApiResponse<string>(true, "Plant issue reported successfully.", null));
        }

        [HttpGet("my-issues")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetMyIssues()
        {
            var userId = GetUserId();
            var issues = await _plantCareService.GetUserIssuesAsync(userId);

            return Ok(new ApiResponse<IEnumerable<PlantIssueViewDto>>(true, "User issues fetched successfully.", issues));
        }

        [HttpGet("all-issues")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllIssues()
        {
            var issues = await _plantCareService.GetAllIssuesAsync();
            return Ok(new ApiResponse<IEnumerable<PlantIssueViewDto>>(true, "All plant issues fetched.", issues));
        }

        [HttpPut("resolve")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ResolveIssue([FromBody] ResolveIssueDto dto)
        {
            var success = await _plantCareService.ResolveIssueAsync(dto);

            if (!success)
                return BadRequest(new ApiResponse<string>(false, "Failed to resolve issue.", null));

            return Ok(new ApiResponse<string>(true, "Issue resolved successfully.", null));
        }

    }
}
