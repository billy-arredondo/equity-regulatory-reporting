using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator(
        IRepository<DocumentType> documentTypeRepository,
        IRepository<Country> countryRepository,
        IRepository<Location> locationRepository,
        IRepository<Person> personRepository)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PersonType).IsInEnum();
        RuleFor(x => x.Ciiu)
            .Empty()
            .When(x => x.PersonType == PersonType.Natural)
            .WithMessage("Natural persons cannot have a CIIU code.");
        RuleFor(x => x.Ciiu)
            .MaximumLength(10)
            .When(x => x.PersonType != PersonType.Natural && x.Ciiu is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
        RuleFor(x => x.DocumentTypeId).NotEmpty();
        RuleFor(x => x.DocumentTypeId)
            .MustAsync(async (id, ct) => await documentTypeRepository.Query().AnyAsync(d => d.Id == id, ct))
            .When(x => x.DocumentTypeId != Guid.Empty)
            .WithMessage("DocumentType not found.");
        RuleFor(x => x.DocumentNumber).MaximumLength(50).When(x => x.DocumentNumber is not null);
        RuleFor(x => x.EntityCode).MaximumLength(50).When(x => x.EntityCode is not null);
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.CountryId)
            .MustAsync(async (id, ct) => await countryRepository.Query().AnyAsync(c => c.Id == id, ct))
            .When(x => x.CountryId != Guid.Empty)
            .WithMessage("Country not found.");
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.LocationId)
            .MustAsync(async (id, ct) => await locationRepository.Query().AnyAsync(l => l.Id == id, ct))
            .When(x => x.LocationId != Guid.Empty)
            .WithMessage("Location not found.");
        RuleFor(x => x.ReportFlag).Equal(false)
            .When(x => x.PersonType == PersonType.Natural)
            .WithMessage("Natural persons cannot be included in the report.");
        RuleFor(x => x.RepresentativeId)
            .Null()
            .When(x => x.PersonType == PersonType.Natural)
            .WithMessage("Natural persons cannot have a representative.");
        RuleFor(x => x.RepresentativeId)
            .MustAsync(async (id, ct) => await personRepository.Query().AnyAsync(p => p.Id == id!.Value, ct))
            .When(x => x.RepresentativeId.HasValue)
            .WithMessage("Representative not found.");
    }
}
