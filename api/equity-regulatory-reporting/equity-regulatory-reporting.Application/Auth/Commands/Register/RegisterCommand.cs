using MediatR;

namespace equity_regulatory_reporting.Application.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<RegisterResult>;

public record RegisterResult(bool Success, string? Error);
