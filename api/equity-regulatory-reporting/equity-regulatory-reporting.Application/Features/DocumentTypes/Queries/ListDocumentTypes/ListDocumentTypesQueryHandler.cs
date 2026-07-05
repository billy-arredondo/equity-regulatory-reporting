using AutoMapper;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Queries.ListDocumentTypes;

public class ListDocumentTypesQueryHandler(IRepository<DocumentType> repository, IMapper mapper)
    : IRequestHandler<ListDocumentTypesQuery, PagedResult<DocumentTypeDto>>
{
    public async Task<PagedResult<DocumentTypeDto>> Handle(ListDocumentTypesQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Query().Include(d => d.AllowedPersonTypes);

        IQueryable<DocumentType> filtered;
        if (string.IsNullOrWhiteSpace(request.Page.Search))
        {
            filtered = query;
        }
        else
        {
            var term = request.Page.Search.ToLower();
            filtered = query.Where(d =>
                d.Name.ToLower().Contains(term) ||
                d.Abbreviation.ToLower().Contains(term));
        }

        var total = await filtered.CountAsync(cancellationToken);

        var items = await filtered
            .OrderBy(d => d.Name)
            .Skip((request.Page.Page - 1) * request.Page.PageSize)
            .Take(request.Page.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<DocumentTypeDto>(
            mapper.Map<List<DocumentTypeDto>>(items),
            request.Page.Page,
            request.Page.PageSize,
            total);
    }
}
