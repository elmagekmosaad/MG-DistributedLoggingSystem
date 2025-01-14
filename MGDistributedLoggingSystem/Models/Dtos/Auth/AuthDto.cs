
using System.Security.Claims;

namespace MGDistributedLoggingSystem.Models.Dtos.Auth
{
    public class AuthDto()
    {
        public string Message { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; } = false;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime? ExpiresOn { get; set; }
        public IEnumerable<Claim>? Claims { get; set; }
    }
}