using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using FluentValidation;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.CreateParticipation;

public class CreateParticipationCommandHandler(
    IRepository<Participation> repository,
    IRepository<Person> personRepository)
    : IRequestHandler<CreateParticipationCommand, Guid>
{
    public async Task<Guid> Handle(CreateParticipationCommand request, CancellationToken cancellationToken)
    {
        var company = await personRepository.GetByIdAsync(request.CompanyId, cancellationToken)
            ?? throw new ValidationException("Company (Person) not found.");

        if (company.PersonType is not (PersonType.Legal or PersonType.LegalEntity))
            throw new ValidationException("Company must be a Legal or LegalEntity person.");

        var participation = new Participation
        {
            CompanyId = request.CompanyId,
            ShareholderId = request.ShareholderId,
            Percentage = request.Percentage,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo
        };

        await repository.AddAsync(participation, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return participation.Id;
    }
}
