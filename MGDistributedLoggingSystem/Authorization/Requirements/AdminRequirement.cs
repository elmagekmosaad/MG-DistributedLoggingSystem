using Microsoft.AspNetCore.Authorization;
using MGDistributedLoggingSystem.Constants;

namespace MGDistributedLoggingSystem.Authorization.Requirements
{
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            if (context.User.IsInRole(Roles.Admin))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
    public class AdminRequirement : IAuthorizationRequirement
    {

    }
}