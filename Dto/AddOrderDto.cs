namespace NestLeaf.Dto
{
    public class AddOrderDto
    {
        public string? PaymentMethod { get; set; }
        public int ShippingAddressId { get; set; }
        public List<AddOrderItemDto> Items { get; set; }
    }
}
