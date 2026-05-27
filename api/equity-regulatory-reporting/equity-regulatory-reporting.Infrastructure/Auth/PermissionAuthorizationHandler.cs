using System.Security.Claims;
using equity_regulatory_reporting.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace equity_regulatory_reporting.Infrastructure.Auth;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permClaim = context.User.FindFirstValue("perm");
        if (permClaim is not null && int.TryParse(permClaim, out var permInt))
        {
            var userPermissions = (Permission)permInt;
            if ((userPermissions & requirement.Permission) != 0)
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
