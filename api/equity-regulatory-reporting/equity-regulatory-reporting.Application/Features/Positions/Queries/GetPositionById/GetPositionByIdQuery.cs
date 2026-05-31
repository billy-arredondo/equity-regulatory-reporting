using equity_regulatory_reporting.Application.Features.Positions.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Queries.GetPositionById;

public record GetPositionByIdQuery(Guid Id) : IRequest<PositionDto>;
