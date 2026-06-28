using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.UpdatePerson;

public class UpdatePersonCommandHandler(
    IRepository<Person> personRepository,
    IRepository<DocumentType> documentTypeRepository)
    : IRequestHandler<UpdatePersonCommand>
{
    public async Task Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Person), request.Id);

        var documentType = await documentTypeRepository.Query()
            .Include(d => d.AllowedPersonTypes)
            .FirstOrDefaultAsync(d => d.Id == request.DocumentTypeId, cancellationToken)
            ?? throw new NotFoundException(nameof(DocumentType), request.DocumentTypeId);

        if (!documentType.AllowedPersonTypes.Any(a => a.PersonType == request.PersonType))
            throw new ValidationException($"DocumentType '{documentType.Name}' is not allowed for PersonType '{request.PersonType}'.");

        if (documentType.ValidationRegex is not null && request.DocumentNumber is not null
            && !System.Text.RegularExpressions.Regex.IsMatch(request.DocumentNumber, documentType.ValidationRegex))
            throw new ValidationException($"DocumentNumber does not match the required format for '{documentType.Name}'.");

        person.Name = request.Name;
        person.PersonType = request.PersonType;
        person.Ciiu = request.Ciiu;
        person.Address = request.Address;
        person.DocumentTypeId = request.DocumentTypeId;
        person.DocumentNumber = request.DocumentNumber;
        person.EntityCode = request.EntityCode;
        person.RepresentativeId = request.RepresentativeId;
        person.ReportFlag = request.ReportFlag;
        person.CountryId = request.CountryId;
        person.LocationId = request.LocationId;

        personRepository.Update(person);
        await personRepository.SaveChangesAsync(cancellationToken);
    }
}
