using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Users.Dtos;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    public async Task<string?> GetEmailAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        return user?.Email;
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

    public async Task<PagedResult<UserDto>> ListUsersAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = userManager.Users;

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Email!.Contains(search) || u.FirstName.Contains(search) || u.LastName.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var users = await query
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            dtos.Add(new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles.ToList()));
        }

        return new PagedResult<UserDto>(dtos, page, pageSize, total);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        return new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles.ToList());
    }

    public async Task<(bool Success, string? Error)> UpdateUserAsync(Guid userId, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return (false, "User not found.");

        user.FirstName = firstName;
        user.LastName = lastName;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<(bool Success, string? Error)> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return (false, "User not found.");

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<(bool Success, string? Error)> AssignRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return (false, "User not found.");

        if (!await roleManager.RoleExistsAsync(role))
            return (false, $"Role '{role}' does not exist.");

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await userManager.AddToRoleAsync(user, role);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<IEnumerable<string>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        return await roleManager.Roles.Select(r => r.Name!).ToListAsync(cancellationToken);
    }
}
