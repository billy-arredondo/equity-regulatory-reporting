using equity_regulatory_reporting.Application.Common.Interfaces;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Queries.ListRoles;

public class ListRolesQueryHandler(IIdentityService identityService)
    : IRequestHandler<ListRolesQuery, IEnumerable<string>>
{
    public async Task<IEnumerable<string>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        return await identityService.GetAllRolesAsync(cancellationToken);
    }
}
