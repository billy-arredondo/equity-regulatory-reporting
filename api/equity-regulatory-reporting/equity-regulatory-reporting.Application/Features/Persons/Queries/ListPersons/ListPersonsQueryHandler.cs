using AutoMapper;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Persons.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Persons.Queries.ListPersons;

public class ListPersonsQueryHandler(IRepository<Person> repository, IMapper mapper)
    : IRequestHandler<ListPersonsQuery, PagedResult<PersonDto>>
{
    public async Task<PagedResult<PersonDto>> Handle(ListPersonsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Person> query = repository.Query()
            .Include(p => p.DocumentType)
            .Include(p => p.Country);

        var filtered = query;

        if (request.PersonType.HasValue)
            filtered = filtered.Where(p => p.PersonType == request.PersonType.Value);

        if (!string.IsNullOrWhiteSpace(request.Page.Search))
        {
            var term = request.Page.Search.ToLower();
            filtered = filtered.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.DocumentNumber.ToLower().Contains(term));
        }

        var total = await filtered.CountAsync(cancellationToken);

        var items = await filtered
            .OrderBy(p => p.Name)
            .Skip((request.Page.Page - 1) * request.Page.PageSize)
            .Take(request.Page.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PersonDto>(
            mapper.Map<List<PersonDto>>(items),
            request.Page.Page,
            request.Page.PageSize,
            total);
    }
}
