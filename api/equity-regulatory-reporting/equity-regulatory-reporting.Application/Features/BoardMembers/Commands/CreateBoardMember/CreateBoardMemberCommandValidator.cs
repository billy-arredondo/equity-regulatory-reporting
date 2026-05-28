using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.CreateBoardMember;

public class CreateBoardMemberCommandValidator : AbstractValidator<CreateBoardMemberCommand>
{
    public CreateBoardMemberCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.PrimaryPositionId).NotEmpty();
    }
}
