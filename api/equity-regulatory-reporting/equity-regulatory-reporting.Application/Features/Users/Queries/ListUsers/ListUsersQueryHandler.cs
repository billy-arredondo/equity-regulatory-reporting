using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Users.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Queries.ListUsers;

public class ListUsersQueryHandler(IIdentityService identityService)
    : IRequestHandler<ListUsersQuery, PagedResult<UserDto>>
{
    public async Task<PagedResult<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        return await identityService.ListUsersAsync(
            request.Page.Page,
            request.Page.PageSize,
            request.Page.Search,
            cancellationToken);
    }
}
