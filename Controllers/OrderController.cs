using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Service;
using System.Security.Claims;
using NestLeaf.Response;
using NestLeaf.Models;

namespace NestLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IorderServices _orderService;

        public OrderController(IorderServices orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

     
        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> PlaceOrder([FromBody] AddOrderDto dto)
        {
            var userId = GetUserId();

            var result = await _orderService.AddOrderAsync(dto, userId);
            if (!result)
                return BadRequest(new ApiResponse<string>(false, "Order creation failed. Check address or product info.", null));

            return Ok(new ApiResponse<string>(true, "Order placed successfully.", null));
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetOrdersByUserId()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetOrders(userId);

            if (orders == null || !orders.Any())
                return NotFound(new ApiResponse<string>(false, "No order found for this user", null));

            return Ok(new ApiResponse<List<OrderDto>>(true, "Orders fetched successfully", orders));
        }

      
        [HttpGet("{orderId}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            int? userId = null;

            if (!User.IsInRole("admin"))
            {
                userId = GetUserId();
            }

            var order = await _orderService.GetOrderById(orderId, userId);

            if (order == null)
                return NotFound(new ApiResponse<string>(false, $"No order found with ID {orderId} for current user..", null));

            return Ok(new ApiResponse<OrderDto>(true, "Order fetched successfully", order));
        }

      
        [HttpPost("make-payment")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> MakePayment([FromBody] PaymentRequestDto dto)
        {
            var userId = GetUserId(); 

            try
            {
                var success = await _orderService.MakePayment(dto, userId);

                if (success)
                    return Ok(new ApiResponse<string>(true, "Payment successful",null));
                else
                    return BadRequest(new ApiResponse<string>(false, "Invalid Order ID or already paid",null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Something went wrong",null));
            }
        }

        


        [HttpPut("CancelOrder/{orderId}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var currentUserId = GetUserId();
            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            int? targetUserId = currentUserRole == "admin" ? null : currentUserId;
            string cancelledBy = currentUserRole;

            var isCancelled = await _orderService.CancelOrder(targetUserId, orderId, cancelledBy);

            if (!isCancelled)
                return NotFound(new ApiResponse<string>(false, "Order not found or already cancelled", null));

            return Ok(new ApiResponse<string>(true, "Order cancelled successfully", null));
        }

       
        [HttpGet("admin/GetAllOrders")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var allOrders = await _orderService.GetAllOrdersAsync();
                return Ok(new ApiResponse<List<AdminOrderDto>>(true, "Orders fetched successfully", allOrders));
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new ApiResponse<string>(false, $"Internal Server Error: {ex.Message}", null));
            }

        }

   
        [HttpPut("admin/UpdateStatus")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateStatus(int orderId, int status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
            try
            {
                if (!result)
                    return NotFound(new ApiResponse<string>(false, "Order not found or update failed", null));

                return Ok(new ApiResponse<string>(true, "Order status updated successfully", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, $"Internal error: {ex.Message}", null));
            }

        }

     
    
    }
}
