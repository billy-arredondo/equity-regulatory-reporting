using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.DeleteParticipation;

public class DeleteParticipationCommandValidator : AbstractValidator<DeleteParticipationCommand>
{
    public DeleteParticipationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
