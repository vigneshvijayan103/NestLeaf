namespace NestLeaf.Dto
{
    public class RazorpayOrderResponse
    {
        public string RazorpayOrderId { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
        public int OrderId { get; set; }
    }
}
