using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.UpdatePosition;

public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ReportCode).NotEmpty().Length(2).Matches(@"^\d{2}$")
            .WithMessage("ReportCode must be exactly 2 digits (e.g. '01').");
    }
}
