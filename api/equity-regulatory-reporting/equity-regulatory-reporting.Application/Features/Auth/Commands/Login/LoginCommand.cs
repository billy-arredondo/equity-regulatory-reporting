using MediatR;

namespace equity_regulatory_reporting.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(bool Success, string? AccessToken, string? RefreshToken, string? Error);
