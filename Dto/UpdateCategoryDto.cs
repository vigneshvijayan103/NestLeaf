namespace NestLeaf.Dto
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        public string? CategoryName { get; set; }

        public string? Description { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
