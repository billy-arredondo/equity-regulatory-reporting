using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.CreateBoardMember;

public class CreateBoardMemberCommandHandler(
    IRepository<BoardMember> repository,
    IRepository<Person> personRepository,
    IRepository<Position> positionRepository)
    : IRequestHandler<CreateBoardMemberCommand, Guid>
{
    public async Task<Guid> Handle(CreateBoardMemberCommand request, CancellationToken cancellationToken)
    {
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

        var boardMember = new BoardMember
        {
            CompanyId = request.CompanyId,
            MemberId = request.MemberId,
            PrimaryPositionId = request.PrimaryPositionId,
            SecondaryPositionId = secondaryPositionId
        };

        await repository.AddAsync(boardMember, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return boardMember.Id;
    }
}
