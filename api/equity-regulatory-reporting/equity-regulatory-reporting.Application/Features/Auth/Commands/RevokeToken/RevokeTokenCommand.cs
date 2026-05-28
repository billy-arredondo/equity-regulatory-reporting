using MediatR;

namespace equity_regulatory_reporting.Application.Features.Auth.Commands.RevokeToken;

public record RevokeTokenCommand(string Token) : IRequest<RevokeTokenResult>;

public record RevokeTokenResult(bool Success);
