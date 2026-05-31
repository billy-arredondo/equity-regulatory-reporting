using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.UpdateBoardMember;

public class UpdateBoardMemberCommandHandler(
    IRepository<BoardMember> repository,
    IRepository<Person> personRepository,
    IRepository<Position> positionRepository)
    : IRequestHandler<UpdateBoardMemberCommand>
{
    public async Task Handle(UpdateBoardMemberCommand request, CancellationToken cancellationToken)
    {
        var boardMember = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(BoardMember), request.Id);

        var company = await personRepository.GetByIdAsync(request.CompanyId, cancellationToken)
            ?? throw new ValidationException("Company (Person) not found.");
        if (company.PersonType is not (PersonType.Legal or PersonType.LegalEntity))
            throw new ValidationException("Company must be a Legal or LegalEntity person.");

        var member = await personRepository.GetByIdAsync(request.MemberId, cancellationToken)
            ?? throw new ValidationException("Member (Person) not found.");
        if (member.PersonType is not PersonType.Natural)
            throw new ValidationException("Member must be a Natural person.");

        var secondaryPositionId = request.SecondaryPositionId
            ?? await positionRepository.Query()
                .Where(p => p.Name == "No position")
                .Select(p => p.Id)
                .FirstOrDefaultAsync(cancellationToken);

        if (secondaryPositionId == Guid.Empty)
            throw new ValidationException("The 'No position' sentinel record is missing from the database.");

        boardMember.CompanyId = request.CompanyId;
        boardMember.MemberId = request.MemberId;
        boardMember.PrimaryPositionId = request.PrimaryPositionId;
        boardMember.SecondaryPositionId = secondaryPositionId;

        repository.Update(boardMember);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
