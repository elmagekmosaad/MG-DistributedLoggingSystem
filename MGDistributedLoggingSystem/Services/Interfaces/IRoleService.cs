using Microsoft.AspNetCore.Identity;
using MGDistributedLoggingSystem.Data.Entities;

namespace MGDistributedLoggingSystem.Services
{
    public interface IRoleService
    {
        Task AddUserToRoleAsync(AppUser user, string role);
        Task AddUserToRolesAsync(AppUser user, IEnumerable<string> roles);
        Task<IdentityResult> CreateAsync(IdentityRole identityRole);
        Task<bool> RoleExistsAsync(string role);
        Task<IdentityRole> FindByNameAsync(string roleName);
        Task AddPermissionClaim(IdentityRole role, string module);
    }
}