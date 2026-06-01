using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Features.Persons.Import;
using equity_regulatory_reporting.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommandHandler(
    IRepository<Person> personRepository,
    IRepository<DocumentType> documentTypeRepository)
    : IRequestHandler<CreatePersonCommand, Guid>
{
    public async Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var documentType = await documentTypeRepository.Query()
            .Include(d => d.AllowedPersonTypes)
            .FirstOrDefaultAsync(d => d.Id == request.DocumentTypeId, cancellationToken)
            ?? throw new ValidationException("DocumentType not found.");

        var ruleErrors = PersonImportRules.Check(
            documentType,
            request.PersonType,
            request.DocumentNumber,
            request.RepresentativeId.HasValue);

        if (ruleErrors.Count > 0)
            throw new ValidationException(ruleErrors[0]);

        var person = new Person
        {
            Name = request.Name,
            PersonType = request.PersonType,
            Ciiu = request.Ciiu,
            Address = request.Address,
            DocumentTypeId = request.DocumentTypeId,
            DocumentNumber = request.DocumentNumber,
            EntityCode = request.EntityCode,
            RepresentativeId = request.RepresentativeId,
            ReportFlag = request.ReportFlag,
            CountryId = request.CountryId,
            InternalLocation = request.InternalLocation
        };

        await personRepository.AddAsync(person, cancellationToken);
        await personRepository.SaveChangesAsync(cancellationToken);

        return person.Id;
    }
}
