using Microsoft.AspNetCore.Authorization;
using MGDistributedLoggingSystem.Constants;

namespace MGDistributedLoggingSystem.Authorization.Requirements
{
    public class UserAuthorizationHandler : AuthorizationHandler<UserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
        {
            if (context.User.IsInRole(Roles.Admin)
                | context.User.IsInRole(Roles.User))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
    public class UserRequirement : IAuthorizationRequirement
    {

    }
}