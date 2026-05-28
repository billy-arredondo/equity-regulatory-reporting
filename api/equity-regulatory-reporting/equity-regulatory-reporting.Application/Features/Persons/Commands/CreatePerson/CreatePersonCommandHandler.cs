using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
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

        if (!documentType.AllowedPersonTypes.Any(a => a.PersonType == request.PersonType))
            throw new ValidationException($"DocumentType '{documentType.Name}' is not allowed for PersonType '{request.PersonType}'.");

        if (request.PersonType is PersonType.Legal or PersonType.LegalEntity && request.RepresentativeId is null)
            throw new ValidationException("A representative is required for Legal and LegalEntity persons.");

        if (request.PersonType is PersonType.Natural && request.RepresentativeId is not null)
            throw new ValidationException("Natural persons cannot have a representative.");

        if (documentType.ValidationRegex is not null
            && !System.Text.RegularExpressions.Regex.IsMatch(request.DocumentNumber, documentType.ValidationRegex))
            throw new ValidationException($"DocumentNumber does not match the required format for '{documentType.Name}'.");

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
