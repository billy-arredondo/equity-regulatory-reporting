using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.DeleteBoardMember;

public class DeleteBoardMemberCommandHandler(IRepository<BoardMember> repository)
    : IRequestHandler<DeleteBoardMemberCommand>
{
    public async Task Handle(DeleteBoardMemberCommand request, CancellationToken cancellationToken)
    {
        var boardMember = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(BoardMember), request.Id);

        repository.Remove(boardMember);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
