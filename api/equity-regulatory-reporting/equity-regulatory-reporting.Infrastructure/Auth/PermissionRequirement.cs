using equity_regulatory_reporting.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace equity_regulatory_reporting.Infrastructure.Auth;

public class PermissionRequirement(Permission permission) : IAuthorizationRequirement
{
    public Permission Permission { get; } = permission;
}
