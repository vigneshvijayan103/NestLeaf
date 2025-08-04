using NestLeaf.Dto;

namespace NestLeaf.Service
{
    public interface IRazorpayService
    {
        public RazorpayOrderResponse CreateOrder(int orderId);
    }
}
