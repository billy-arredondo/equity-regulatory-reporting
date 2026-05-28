using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.DeleteBoardMember;

public class DeleteBoardMemberCommandValidator : AbstractValidator<DeleteBoardMemberCommand>
{
    public DeleteBoardMemberCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
