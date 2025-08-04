namespace NestLeaf.Dto
{
    public class RazorpayVerificationDto
    {
        public string RazorpayPaymentId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpaySignature { get; set; }
    }
}
