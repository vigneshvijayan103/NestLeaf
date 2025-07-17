namespace NestLeaf.Dto
{
    public class ViewCartDto
    {
        public int UserId { get; set; }

        public decimal CartTotalPrice { get; set; }

        public List<CartItemDto> items { get; set; }=new List<CartItemDto>();
    }
}
