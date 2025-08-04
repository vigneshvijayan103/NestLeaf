using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Service;

namespace NestLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly RazorpayService _razorpay;

        public PaymentController(RazorpayService razorpay)
        {
            _razorpay = razorpay;
        }

        [HttpGet("create-razorpay-order/{orderId}")]
        public async Task<IActionResult> CreateOrder(int orderId)
        {
            try
            {
                var (razorpayOrderId, amount) = await _razorpay.CreateRazorpayOrder(orderId);
                return Ok(new
                {
                    success = true,
                    razorpayOrderId,
                    amount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] RazorpayVerificationDto dto, [FromQuery] int orderId)
        {
            try
            {
                var isValid = await _razorpay.VerifySignature(dto, orderId);
                if (isValid)
                {
                    return Ok(new { success = true, message = "Payment verified and order marked as paid." });
                }
                return BadRequest(new { success = false, message = "Invalid payment signature." });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

