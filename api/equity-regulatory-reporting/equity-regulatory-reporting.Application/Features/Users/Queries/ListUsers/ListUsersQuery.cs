using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Users.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Queries.ListUsers;

public record ListUsersQuery(PageRequest Page) : IRequest<PagedResult<UserDto>>;
