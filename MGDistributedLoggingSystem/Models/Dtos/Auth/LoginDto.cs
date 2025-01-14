namespace MGDistributedLoggingSystem.Models.Dtos.Auth
{
    public class LoginDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
        public bool RememberMe { get; set; }


    }
}