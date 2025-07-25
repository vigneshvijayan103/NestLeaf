using NestLeaf.Enum;

namespace NestLeaf.Dto
{
    public class UpdateOrderStatusDto
    {
    
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
    }
}
