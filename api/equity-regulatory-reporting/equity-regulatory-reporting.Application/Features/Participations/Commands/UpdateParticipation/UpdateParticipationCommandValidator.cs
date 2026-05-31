using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.UpdateParticipation;

public class UpdateParticipationCommandValidator : AbstractValidator<UpdateParticipationCommand>
{
    public UpdateParticipationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.ShareholderId).NotEmpty();
        RuleFor(x => x.Percentage).InclusiveBetween(0, 100);
        RuleFor(x => x.EffectiveFrom).NotEmpty();
        RuleFor(x => x.EffectiveTo)
            .GreaterThanOrEqualTo(x => x.EffectiveFrom)
            .When(x => x.EffectiveTo.HasValue)
            .WithMessage("EffectiveTo must be greater than or equal to EffectiveFrom.");
    }
}
