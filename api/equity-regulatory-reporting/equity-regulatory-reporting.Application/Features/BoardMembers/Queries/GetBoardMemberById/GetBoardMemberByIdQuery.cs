using equity_regulatory_reporting.Application.Features.BoardMembers.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Queries.GetBoardMemberById;

public record GetBoardMemberByIdQuery(Guid Id) : IRequest<BoardMemberDto>;
