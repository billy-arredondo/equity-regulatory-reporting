using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IIdentityService identityService)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var (success, _) = await identityService.DeleteUserAsync(request.Id, cancellationToken);
        if (!success)
            throw new NotFoundException("User", request.Id);
    }
}
