using equity_regulatory_reporting.Application.Common.Interfaces;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommandHandler(IRefreshTokenService refreshTokenService)
    : IRequestHandler<RevokeTokenCommand, RevokeTokenResult>
{
    public async Task<RevokeTokenResult> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        await refreshTokenService.RevokeAsync(request.Token);
        return new RevokeTokenResult(true);
    }
}
