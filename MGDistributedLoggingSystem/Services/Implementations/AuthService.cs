using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MGDistributedLoggingSystem.Constants;
using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Helpers;
using MGDistributedLoggingSystem.Models.Dtos.Auth;
using MGDistributedLoggingSystem.Services.Interfaces;

namespace MGDistributedLoggingSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        public AuthService(UserManager<AppUser> userManager, IRoleService roleService, ITokenService tokenService, IMapper mapper)
        {
            this._userManager = userManager;
            this._roleService = roleService;
            this._tokenService = tokenService;
            this._mapper = mapper;
        }
        public async Task<BaseResponse> LogIn(LoginDto loginDto)
        {
            BaseResponse result;
            AppUser? user = loginDto.Email.Contains('@')
                                        ? await _userManager.FindByEmailAsync(loginDto.Email)
                                        : await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null)
            {
                result = new BaseResponse(succeeded: false, message: "invalid email or username", data: null);
                return result;
            }

            if (await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                AuthDto authDto = await _tokenService.GenerateToken(user);

                result = new BaseResponse(succeeded: true, message: "Successfully Logged In", data: authDto);
                return result;
            }
            else
            {
                result = new BaseResponse(succeeded: false, message: "invalid password", data: null);
                return result;
            }

        }

        public async Task<BaseResponse> Register(RegisterDto registerDto)
        {
            AppUser user = _mapper.Map<AppUser>(registerDto);

            if (await _userManager.FindByEmailAsync(user.Email) is not null)
            {
                return new(message: "Email Already Registered");
            }
            if (await _userManager.FindByEmailAsync(user.UserName) is not null)
            {
                return new(message: "UserName Already Registered");
            }

            IdentityResult identity = await _userManager.CreateAsync(user, registerDto.Password);
            if (identity.Succeeded)
            {
                await _roleService.AddUserToRoleAsync(user, Roles.User);
                var token = await _tokenService.GenerateToken(user);

                return new BaseResponse(succeeded: true, message: "Successfully Registered", data: token);
            }
            else
            {
                return new BaseResponse(succeeded: false, message: "Something went wrong", data: identity.Errors);
            }
        }
    }
}
