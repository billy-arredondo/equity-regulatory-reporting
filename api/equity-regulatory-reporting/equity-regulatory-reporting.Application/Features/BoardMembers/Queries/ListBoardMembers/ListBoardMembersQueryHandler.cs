using AutoMapper;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.BoardMembers.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Queries.ListBoardMembers;

public class ListBoardMembersQueryHandler(IRepository<BoardMember> repository, IMapper mapper)
    : IRequestHandler<ListBoardMembersQuery, PagedResult<BoardMemberDto>>
{
    public async Task<PagedResult<BoardMemberDto>> Handle(ListBoardMembersQuery request, CancellationToken cancellationToken)
    {
        IQueryable<BoardMember> query = repository.Query()
            .Include(b => b.Company)
            .Include(b => b.Member)
            .Include(b => b.PrimaryPosition)
            .Include(b => b.SecondaryPosition);

        var filtered = query;

        if (request.CompanyId.HasValue)
            filtered = filtered.Where(b => b.CompanyId == request.CompanyId.Value);

        if (!string.IsNullOrWhiteSpace(request.Page.Search))
            filtered = filtered.Where(b =>
                b.Company.Name.Contains(request.Page.Search) ||
                b.Member.Name.Contains(request.Page.Search));

        var total = await filtered.CountAsync(cancellationToken);

        var items = await filtered
            .OrderBy(b => b.Member.Name)
            .Skip((request.Page.Page - 1) * request.Page.PageSize)
            .Take(request.Page.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BoardMemberDto>(
            mapper.Map<List<BoardMemberDto>>(items),
            request.Page.Page,
            request.Page.PageSize,
            total);
    }
}
