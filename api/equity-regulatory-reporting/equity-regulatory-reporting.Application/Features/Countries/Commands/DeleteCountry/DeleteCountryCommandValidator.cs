using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.DeleteCountry;

public class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
{
    public DeleteCountryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
