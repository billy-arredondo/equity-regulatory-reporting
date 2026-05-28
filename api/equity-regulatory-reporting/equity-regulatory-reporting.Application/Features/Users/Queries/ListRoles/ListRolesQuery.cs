using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Queries.ListRoles;

public record ListRolesQuery : IRequest<IEnumerable<string>>;
