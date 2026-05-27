using System.Security.Cryptography;
using System.Text;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Infrastructure.Options;
using equity_regulatory_reporting.Persistence;
using equity_regulatory_reporting.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace equity_regulatory_reporting.Infrastructure.Services;

public class RefreshTokenService(ApplicationDbContext context, IOptions<JwtOptions> options) : IRefreshTokenService
{
    private readonly JwtOptions _opts = options.Value;

    public async Task<string> IssueAsync(Guid userId)
    {
        var rawToken = GenerateRawToken();
        var tokenHash = Hash(rawToken);

        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_opts.RefreshTokenDays)
        });

        await context.SaveChangesAsync();
        return rawToken;
    }

    public async Task<(bool Valid, Guid UserId, string NewRawToken)> RotateAsync(string rawToken)
    {
        var tokenHash = Hash(rawToken);
        var stored = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

        if (stored is null || !stored.IsActive)
            return (false, Guid.Empty, string.Empty);

        var newRawToken = GenerateRawToken();
        var newHash = Hash(newRawToken);
        var newToken = new RefreshToken
        {
            UserId = stored.UserId,
            TokenHash = newHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_opts.RefreshTokenDays)
        };

        stored.RevokedAt = DateTimeOffset.UtcNow;
        stored.ReplacedByTokenId = newToken.Id;

        context.RefreshTokens.Add(newToken);
        await context.SaveChangesAsync();

        return (true, stored.UserId, newRawToken);
    }

    public async Task RevokeAsync(string rawToken)
    {
        var tokenHash = Hash(rawToken);
        var stored = await context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        if (stored is null || stored.RevokedAt.HasValue) return;

        stored.RevokedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();
    }

    private static string GenerateRawToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
