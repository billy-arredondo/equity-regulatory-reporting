using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Positions.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Queries.ListPositions;

public record ListPositionsQuery(PageRequest Page) : IRequest<PagedResult<PositionDto>>;
