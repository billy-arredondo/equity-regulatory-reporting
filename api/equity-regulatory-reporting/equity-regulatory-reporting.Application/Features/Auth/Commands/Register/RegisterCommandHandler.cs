using equity_regulatory_reporting.Application.Common.Interfaces;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (success, error) = await identityService.RegisterAsync(
            request.Email, request.Password, request.FirstName, request.LastName);

        return new RegisterResult(success, error);
    }
}
