using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.BoardMembers.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Queries.ListBoardMembers;

public record ListBoardMembersQuery(PageRequest Page, Guid? CompanyId = null) : IRequest<PagedResult<BoardMemberDto>>;
