    namespace NestLeaf.Dto
{
    public class PaginatedResult<T>
    {
        public List <T> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
 