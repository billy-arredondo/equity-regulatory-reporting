namespace equity_regulatory_reporting.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<string> IssueAsync(Guid userId);
    Task<(bool Valid, Guid UserId, string NewRawToken)> RotateAsync(string rawToken);
    Task RevokeAsync(string rawToken);
}
