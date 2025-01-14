using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Models.Dtos.Auth;

namespace MGDistributedLoggingSystem.Services
{
    public interface ITokenService
    {
        Task<AuthDto> GenerateToken(AppUser user);
    }
}
