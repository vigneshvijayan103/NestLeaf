namespace NestLeaf.Dto
{
    public class PlantIssueViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Resolution { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProductName { get; set; }
        public string UserName { get; set; }
    }
}
