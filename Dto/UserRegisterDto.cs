using System.ComponentModel.DataAnnotations;

namespace NestLeaf.Dto
{
    public class UserRegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]

        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(20, MinimumLength = 10)]
        public string PhoneNumber { get; set; }
    }
}
