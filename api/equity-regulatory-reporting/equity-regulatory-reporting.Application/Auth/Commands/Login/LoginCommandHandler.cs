using equity_regulatory_reporting.Application.Common.Interfaces;
using MediatR;

namespace equity_regulatory_reporting.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    IIdentityService identityService,
    IJwtTokenService jwtTokenService,
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, error) = await identityService.ValidateCredentialsAsync(request.Email, request.Password);
        if (!success)
            return new LoginResult(false, null, null, error);

        var roles = (await identityService.GetRolesAsync(userId)).ToList();
        var permissions = await identityService.GetEffectivePermissionsAsync(userId);

        var accessToken = jwtTokenService.GenerateAccessToken(userId, request.Email, roles, permissions);
        var refreshToken = await refreshTokenService.IssueAsync(userId);

        return new LoginResult(true, accessToken, refreshToken, null);
    }
}
