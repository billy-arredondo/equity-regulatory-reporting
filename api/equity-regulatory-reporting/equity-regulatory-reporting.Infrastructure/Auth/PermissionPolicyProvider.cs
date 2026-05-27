using equity_regulatory_reporting.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace equity_regulatory_reporting.Infrastructure.Auth;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : IAuthorizationPolicyProvider
{
    private const string PermissionPrefix = "Permission:";
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PermissionPrefix, StringComparison.OrdinalIgnoreCase))
            return _fallback.GetPolicyAsync(policyName);

        var permValue = policyName[PermissionPrefix.Length..];
        if (!int.TryParse(permValue, out var intValue))
            return _fallback.GetPolicyAsync(policyName);

        var permission = (Permission)intValue;
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permission))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => _fallback.GetFallbackPolicyAsync();
}
