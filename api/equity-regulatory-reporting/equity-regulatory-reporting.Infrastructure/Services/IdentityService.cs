using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace equity_regulatory_reporting.Infrastructure.Services;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IIdentityService
{
    public async Task<(bool Success, string? Error)> RegisterAsync(
        string email, string password, string firstName, string lastName)
    {
        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = email,
            Email = email,
            EmailConfirmed = false,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }

    public async Task<(bool Success, Guid UserId, string? Error)> ValidateCredentialsAsync(
        string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return (false, Guid.Empty, "Invalid credentials.");

        var valid = await userManager.CheckPasswordAsync(user, password);
        return valid
            ? (true, user.Id, null)
            : (false, Guid.Empty, "Invalid credentials.");
    }

    public async Task<IEnumerable<string>> GetRolesAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return [];
        return await userManager.GetRolesAsync(user);
    }

    public async Task<Permission> GetEffectivePermissionsAsync(Guid userId)
    {
        var roleNames = await GetRolesAsync(userId);
        var permissions = Permission.None;

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is not null)
                permissions |= role.Permissions;
        }

        return permissions;
    }
}
