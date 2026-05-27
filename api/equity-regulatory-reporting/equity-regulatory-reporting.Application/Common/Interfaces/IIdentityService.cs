using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<(bool Success, Guid UserId, string? Error)> ValidateCredentialsAsync(string email, string password);
    Task<IEnumerable<string>> GetRolesAsync(Guid userId);
    Task<Permission> GetEffectivePermissionsAsync(Guid userId);
}
