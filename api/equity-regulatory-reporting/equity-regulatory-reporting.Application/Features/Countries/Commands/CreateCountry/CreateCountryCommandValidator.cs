using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.CreateCountry;

public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Abbreviation).NotEmpty().MaximumLength(10);
    }
}
