 using Microsoft.AspNetCore.Identity;
using MGDistributedLoggingSystem.Constants;
using MGDistributedLoggingSystem.Services;

namespace MGDistributedLoggingSystem.Seeds
{
    public class DefaultRoles
    {
        private readonly IRoleService roleService;
        public DefaultRoles(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public async Task Initialize()
        {
            await rolesSeeding();
        }

        private async Task rolesSeeding()
        {
            foreach (var role in Roles.ToList())
            {
                if (!await roleService.RoleExistsAsync(role))
                {
                    await roleService.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
