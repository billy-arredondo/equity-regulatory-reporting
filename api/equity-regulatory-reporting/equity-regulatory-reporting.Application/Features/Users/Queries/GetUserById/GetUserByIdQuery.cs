using equity_regulatory_reporting.Application.Features.Users.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
