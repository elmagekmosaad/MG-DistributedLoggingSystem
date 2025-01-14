using MGDistributedLoggingSystem.Helpers;
using MGDistributedLoggingSystem.Models.Dtos.Auth;

namespace MGDistributedLoggingSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse> LogIn(LoginDto user);
        Task<BaseResponse> Register(RegisterDto user);
    }
}
