using MediatR;

namespace equity_regulatory_reporting.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : IRequest<RefreshTokenResult>;

public record RefreshTokenResult(bool Success, string? AccessToken, string? RefreshToken, string? Error);
