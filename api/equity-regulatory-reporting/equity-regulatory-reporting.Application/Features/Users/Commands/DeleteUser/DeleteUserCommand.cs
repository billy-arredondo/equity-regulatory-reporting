using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
