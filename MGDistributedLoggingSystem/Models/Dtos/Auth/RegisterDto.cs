using System.ComponentModel.DataAnnotations;

namespace MGDistributedLoggingSystem.Models.Dtos.Auth
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }

        [Required, EmailAddress(ErrorMessage = "Enter a valid Email Address")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}