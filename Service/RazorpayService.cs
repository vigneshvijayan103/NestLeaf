using Dapper;
using NestLeaf.Dto;
using Razorpay.Api;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using NestLeaf.Models;
namespace NestLeaf.Service
{
    public class RazorpayService
    {
        private readonly IConfiguration _config;
        private readonly IDbConnection _db;

        public RazorpayService(IConfiguration config, IDbConnection db)
        {
            _config = config;
            _db = db;
        }

        public async Task<(string RazorpayOrderId, int Amount)> CreateRazorpayOrder(int orderId)
        {
            try
            {
                var order = await _db.QueryFirstOrDefaultAsync<NestLeaf.Models.Order>(
                    "SELECT * FROM orders WHERE Id = @OrderId", new { OrderId = orderId });

                if (order == null || order.IsPaid)
                    throw new Exception("Invalid or already paid order");

                var key = _config["Razorpay:Key"];
                var secret = _config["Razorpay:Secret"];

                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
                    throw new Exception("Razorpay credentials are missing from configuration.");

                var client = new RazorpayClient(key, secret);

                int amountInPaise = (int)(order.TotalAmount * 100);

                var options = new Dictionary<string, object>
        {
            { "amount", amountInPaise },
            { "currency", "INR" },
            { "receipt", $"order_rcptid_{orderId}" },
            { "payment_capture", 1 }
        };

                var razorOrder = client.Order.Create(options);

                if (razorOrder == null)
                    throw new Exception("Failed to create Razorpay order.");

                return (razorOrder["id"].ToString(), amountInPaise);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Razorpay order creation: {ex.Message}", ex);
            }

        }

        public async Task<bool> VerifySignature(RazorpayVerificationDto dto, int orderId)
        {
            string secret = _config["Razorpay:Secret"];
            string payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
            string expectedSignature;

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                expectedSignature = Convert.ToHexString(hash).ToLower();
            }

            bool isValid = expectedSignature == dto.RazorpaySignature?.Trim(); 


            Console.WriteLine($"Expected Signature: {expectedSignature}");
            Console.WriteLine($"Razorpay Signature: {dto.RazorpaySignature}");

            var parameters = new DynamicParameters();
            parameters.Add("@RazorpayOrderId", dto.RazorpayOrderId);
            parameters.Add("@RazorpayPaymentId", dto.RazorpayPaymentId);
            parameters.Add("@RazorpaySignature", dto.RazorpaySignature);
            parameters.Add("@Verified", isValid);
            parameters.Add("@OrderId", orderId);

            await _db.ExecuteAsync("sp_VerifyRazorpayPayment", parameters, commandType: CommandType.StoredProcedure);


            if (isValid)
            {
                await _db.ExecuteAsync("UPDATE Orders SET IsPaid = 1 ,UpdatedAt=Getdate() WHERE Id  = @OrderId", new { OrderId = orderId });
            }
           

            return isValid;
        }
    }
    
}

