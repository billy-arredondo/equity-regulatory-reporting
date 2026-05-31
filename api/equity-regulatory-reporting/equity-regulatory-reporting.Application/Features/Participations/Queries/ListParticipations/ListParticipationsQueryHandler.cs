using AutoMapper;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Participations.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Participations.Queries.ListParticipations;

public class ListParticipationsQueryHandler(IRepository<Participation> repository, IMapper mapper)
    : IRequestHandler<ListParticipationsQuery, PagedResult<ParticipationDto>>
{
    public async Task<PagedResult<ParticipationDto>> Handle(ListParticipationsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Participation> query = repository.Query()
            .Include(p => p.Company)
            .Include(p => p.Shareholder);

        var filtered = query;

        if (request.CompanyId.HasValue)
            filtered = filtered.Where(p => p.CompanyId == request.CompanyId.Value);

        if (!string.IsNullOrWhiteSpace(request.Page.Search))
            filtered = filtered.Where(p =>
                p.Company.Name.Contains(request.Page.Search) ||
                p.Shareholder.Name.Contains(request.Page.Search));

        var total = await filtered.CountAsync(cancellationToken);

        var items = await filtered
            .OrderByDescending(p => p.EffectiveFrom)
            .Skip((request.Page.Page - 1) * request.Page.PageSize)
            .Take(request.Page.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ParticipationDto>(
            mapper.Map<List<ParticipationDto>>(items),
            request.Page.Page,
            request.Page.PageSize,
            total);
    }
}
