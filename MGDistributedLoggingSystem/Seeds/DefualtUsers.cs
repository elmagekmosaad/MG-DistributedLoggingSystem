using AutoMapper;
using MGDistributedLoggingSystem.Services;
using Microsoft.AspNetCore.Identity;
using MGDistributedLoggingSystem.Constants;
using MGDistributedLoggingSystem.Data.Entities;

namespace MGDistributedLoggingSystem.Seeds
{
    public class DefaultUsers
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IRoleService roleService;

        public DefaultUsers(UserManager<AppUser> userManager, IRoleService roleService)
        {
            this.userManager = userManager;
            this.roleService = roleService;
        }
        public async Task Initialize()
        {
            await defaultAdmin();
            await defaultUser();
        }

        private async Task defaultAdmin()
        {
            var defaultAdmin = new AppUser
            {
                EmailConfirmed = true,
                UserName = DefaultAdmin.UserName,
                Email = DefaultAdmin.Email,
                PhoneNumber = DefaultAdmin.PhoneNumber,
            };

            defaultAdmin = await CreateUserAsync(defaultAdmin, DefaultAdmin.Password);
            await roleService.AddUserToRolesAsync(defaultAdmin, [Roles.Admin, Roles.User]);
        }
        private async Task defaultUser()
        {
            var defaultUser = new AppUser
            {
                EmailConfirmed = true,
                UserName = DefaultUser.UserName,
                Email = DefaultUser.Email,
                PhoneNumber = DefaultUser.PhoneNumber,
            };

            defaultUser = await CreateUserAsync(defaultUser, DefaultUser.Password);
            await roleService.AddUserToRoleAsync(defaultUser, Roles.User);
        }
        private async Task<AppUser> CreateUserAsync(AppUser user, string password)
        {
            AppUser result = await userManager.FindByEmailAsync(user.Email);
            if (result is null)
            {
                await userManager.CreateAsync(user, password);
                return user;
            }
            return result;
        }

    }
}
