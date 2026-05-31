using AutoMapper;
using AutoMapper.QueryableExtensions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Positions.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Positions.Queries.ListPositions;

public class ListPositionsQueryHandler(IRepository<Position> repository, IMapper mapper)
    : IRequestHandler<ListPositionsQuery, PagedResult<PositionDto>>
{
    public async Task<PagedResult<PositionDto>> Handle(ListPositionsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Query();

        if (!string.IsNullOrWhiteSpace(request.Page.Search))
            query = query.Where(p => p.Name.Contains(request.Page.Search));

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((request.Page.Page - 1) * request.Page.PageSize)
            .Take(request.Page.PageSize)
            .ProjectTo<PositionDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<PositionDto>(items, request.Page.Page, request.Page.PageSize, total);
    }
}
