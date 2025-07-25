using NestLeaf.Enum;

namespace NestLeaf.Dto
{
    public class AdminOrderDto
    {
        public int OrderId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }


}


