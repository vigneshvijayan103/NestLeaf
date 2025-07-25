using NestLeaf.Enum;
namespace NestLeaf.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string PaymentMethod { get; set; }
        public int ShippingAddressId { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
