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
                return BadRequest("Order creation failed. Check address or product info.");

            return Ok("Order placed successfully.");
        }

       
        [HttpGet]
        [Authorize(Roles = "user")]

        public async Task<IActionResult> GetOrdersByUserId()
        {
            var userid = GetUserId();
            var orders = await _orderService.GetOrders(userid);

            if (orders == null || !orders.Any())
                return NotFound(new { Message = "No orders found for this user." });

            return Ok(orders);

           
        }

      
        [HttpGet("{orderId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {

            var userid = GetUserId();


            var order = await _orderService.GetOrderById(orderId, userid);

                if (order == null)
                    return NotFound(new { Message = "Order not found." });

                return Ok(order);
            
            
        }

    }
}