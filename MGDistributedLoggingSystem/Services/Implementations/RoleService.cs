using Microsoft.AspNetCore.Identity;
using System.Security.Claims; 
using MGDistributedLoggingSystem.Constants;
using MGDistributedLoggingSystem.Data.Entities;

namespace MGDistributedLoggingSystem.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task AddUserToRoleAsync(AppUser user, string role)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
        public async Task AddUserToRolesAsync(AppUser user, IEnumerable<string> roles)
        {
            await userManager.AddToRolesAsync(user, roles); 
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole identityRole)
        {
            return await roleManager.CreateAsync(identityRole);
        }

        public async Task<IdentityRole> FindByNameAsync(string roleName)
        {
            return await roleManager.FindByNameAsync(roleName);
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await roleManager.RoleExistsAsync(role);
        }
        public async Task AddPermissionClaim(IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == Constants.Constants.Permission && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim(Constants.Constants.Permission, permission));
                }
            }
        }
    }
}
