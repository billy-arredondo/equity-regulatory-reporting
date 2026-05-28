using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.UpdateBoardMember;

public class UpdateBoardMemberCommandValidator : AbstractValidator<UpdateBoardMemberCommand>
{
    public UpdateBoardMemberCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.PrimaryPositionId).NotEmpty();
    }
}
