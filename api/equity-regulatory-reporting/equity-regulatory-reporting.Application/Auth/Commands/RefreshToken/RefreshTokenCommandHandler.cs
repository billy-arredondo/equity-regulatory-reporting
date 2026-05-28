using equity_regulatory_reporting.Application.Common.Interfaces;
using MediatR;

namespace equity_regulatory_reporting.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IIdentityService identityService,
    IJwtTokenService jwtTokenService,
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var (valid, userId, newRawToken) = await refreshTokenService.RotateAsync(request.Token);
        if (!valid)
            return new RefreshTokenResult(false, null, null, "Invalid or expired refresh token.");

        var email = (await identityService.GetRolesAsync(userId)).FirstOrDefault() ?? string.Empty;
        var roles = (await identityService.GetRolesAsync(userId)).ToList();
        var permissions = await identityService.GetEffectivePermissionsAsync(userId);

        var accessToken = jwtTokenService.GenerateAccessToken(userId, email, roles, permissions);

        return new RefreshTokenResult(true, accessToken, newRawToken, null);
    }
}
