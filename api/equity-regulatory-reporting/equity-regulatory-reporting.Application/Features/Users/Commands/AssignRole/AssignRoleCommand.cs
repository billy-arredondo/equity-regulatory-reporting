using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Commands.AssignRole;

public record AssignRoleCommand(Guid UserId, string Role) : IRequest;
