using AutoMapper; 
using Microsoft.AspNetCore.Identity;
using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Services;

namespace MGDistributedLoggingSystem.Seeds
{
    public class SeedData
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IRoleService roleService;


        public SeedData(UserManager<AppUser> userManager, IRoleService roleService)
        {
            this.userManager = userManager;
            this.roleService = roleService;
        }

        public async Task InitializeAsync()
        {
            await new DefaultRoles(roleService).Initialize();
            await new DefaultUsers(userManager, roleService).Initialize();
        }
    }
}