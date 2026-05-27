using MediatR;

namespace equity_regulatory_reporting.Application.Auth.Commands.RevokeToken;

public record RevokeTokenCommand(string Token) : IRequest<RevokeTokenResult>;

public record RevokeTokenResult(bool Success);
