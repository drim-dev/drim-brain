using System.Security.Claims;
using Auth.Features.Auth.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Authorization.Requirements;

public class RoleRequirement : IAuthorizationRequirement
{
    public RoleRequirement(UserRole requiredRole)
    {
        RequiredRole = requiredRole;
    }

    public UserRole RequiredRole { get; }
}

public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        if (context.User.HasClaim(ClaimTypes.Role, requirement.RequiredRole.ToString()))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class ActionRequirement : IAuthorizationRequirement
{
    public ActionRequirement(string actionName)
    {
        ActionName = actionName;
    }

    public string ActionName { get; set; }
}

public class ActionRequirementHandler : AuthorizationHandler<ActionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActionRequirement requirement)
    {
        throw new NotImplementedException();
    }
}
