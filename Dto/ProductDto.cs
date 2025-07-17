namespace NestLeaf.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = null!;

        public int CategoryId { get; set; }

    }
}
