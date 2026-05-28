using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Users.Dtos;
using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<(bool Success, Guid UserId, string? Error)> ValidateCredentialsAsync(string email, string password);
    Task<string?> GetEmailAsync(Guid userId);
    Task<IEnumerable<string>> GetRolesAsync(Guid userId);
    Task<Permission> GetEffectivePermissionsAsync(Guid userId);

    Task<PagedResult<UserDto>> ListUsersAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> UpdateUserAsync(Guid userId, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> AssignRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAllRolesAsync(CancellationToken cancellationToken = default);
}
