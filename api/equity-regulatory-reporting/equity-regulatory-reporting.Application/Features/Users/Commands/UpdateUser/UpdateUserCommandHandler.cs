using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IIdentityService identityService)
    : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var (success, error) = await identityService.UpdateUserAsync(request.Id, request.FirstName, request.LastName, cancellationToken);
        if (!success)
            throw new NotFoundException("User", request.Id);
    }
}
