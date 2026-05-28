using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName) : IRequest;
