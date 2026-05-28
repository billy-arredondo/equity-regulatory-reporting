using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using FluentValidation;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.UpdateParticipation;

public class UpdateParticipationCommandHandler(
    IRepository<Participation> repository,
    IRepository<Person> personRepository)
    : IRequestHandler<UpdateParticipationCommand>
{
    public async Task Handle(UpdateParticipationCommand request, CancellationToken cancellationToken)
    {
        var participation = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Participation), request.Id);

        var company = await personRepository.GetByIdAsync(request.CompanyId, cancellationToken)
            ?? throw new ValidationException("Company (Person) not found.");

        if (company.PersonType is not (PersonType.Legal or PersonType.LegalEntity))
            throw new ValidationException("Company must be a Legal or LegalEntity person.");

        participation.CompanyId = request.CompanyId;
        participation.ShareholderId = request.ShareholderId;
        participation.Percentage = request.Percentage;
        participation.EffectiveFrom = request.EffectiveFrom;
        participation.EffectiveTo = request.EffectiveTo;

        repository.Update(participation);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
