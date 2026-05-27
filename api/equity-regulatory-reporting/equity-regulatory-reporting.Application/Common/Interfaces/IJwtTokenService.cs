using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles, Permission permissions);
    Guid? GetUserIdFromToken(string token);
}
