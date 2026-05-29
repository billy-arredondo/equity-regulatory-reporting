using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ReportCode).NotEmpty().Length(2).Matches(@"^\d{2}$")
            .WithMessage("ReportCode must be exactly 2 digits (e.g. '01').");
    }
}
