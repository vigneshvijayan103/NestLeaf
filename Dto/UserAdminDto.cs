namespace NestLeaf.Dto
{
    public class UserAdminDto
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
