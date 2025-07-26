namespace NestLeaf.Dto
{
    public class AddOrderDto
    {
      
        public int ShippingAddressId { get; set; }
        public List<AddOrderItemDto> Items { get; set; }
    }
}
