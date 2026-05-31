using equity_regulatory_reporting.Domain.Enums;
using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.UpdatePerson;

public class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PersonType).IsInEnum();
        RuleFor(x => x.Ciiu).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DocumentTypeId).NotEmpty();
        RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EntityCode).MaximumLength(50).When(x => x.EntityCode is not null);
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.InternalLocation).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ReportFlag).Equal(false)
            .When(x => x.PersonType == PersonType.Natural)
            .WithMessage("Natural persons cannot be included in the report.");
    }
}
