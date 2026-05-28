using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Features.Users.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await identityService.GetUserByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        return user;
    }
}
