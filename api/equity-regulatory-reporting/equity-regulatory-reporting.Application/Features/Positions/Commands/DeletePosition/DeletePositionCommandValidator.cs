using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.DeletePosition;

public class DeletePositionCommandValidator : AbstractValidator<DeletePositionCommand>
{
    public DeletePositionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
