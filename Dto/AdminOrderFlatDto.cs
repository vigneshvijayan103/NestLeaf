using NestLeaf.Enum;

namespace NestLeaf.Dto
{
    public class AdminOrderFlatDto
    {
        public int OrderId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public OrderStatus Status { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
