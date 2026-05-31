using equity_regulatory_reporting.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Commands.AssignRole;

public class AssignRoleCommandHandler(IIdentityService identityService)
    : IRequestHandler<AssignRoleCommand>
{
    public async Task Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var (success, error) = await identityService.AssignRoleAsync(request.UserId, request.Role, cancellationToken);
        if (!success)
            throw new ValidationException(error ?? "Failed to assign role.");
    }
}
