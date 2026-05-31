using AutoMapper;
using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Features.BoardMembers.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Queries.GetBoardMemberById;

public class GetBoardMemberByIdQueryHandler(IRepository<BoardMember> repository, IMapper mapper)
    : IRequestHandler<GetBoardMemberByIdQuery, BoardMemberDto>
{
    public async Task<BoardMemberDto> Handle(GetBoardMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var boardMember = await repository.Query()
            .Include(b => b.Company)
            .Include(b => b.Member)
            .Include(b => b.PrimaryPosition)
            .Include(b => b.SecondaryPosition)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(BoardMember), request.Id);

        return mapper.Map<BoardMemberDto>(boardMember);
    }
}
