namespace NestLeaf.Dto
{
    public class CategoryViewDto
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
