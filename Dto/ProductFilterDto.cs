namespace NestLeaf.Dto
{
    public class ProductFilterDto
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
