using AutoMapper;
using AutoMapper.QueryableExtensions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Countries.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Countries.Queries.ListCountries;

public class ListCountriesQueryHandler(IRepository<Country> repository, IMapper mapper)
    : IRequestHandler<ListCountriesQuery, PagedResult<CountryDto>>
{
    public async Task<PagedResult<CountryDto>> Handle(ListCountriesQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Query();

        if (!string.IsNullOrWhiteSpace(request.Page.Search))
            query = query.Where(c => c.Name.Contains(request.Page.Search) || c.Abbreviation.Contains(request.Page.Search));

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((request.Page.Page - 1) * request.Page.PageSize)
            .Take(request.Page.PageSize)
            .ProjectTo<CountryDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<CountryDto>(items, request.Page.Page, request.Page.PageSize, total);
    }
}
