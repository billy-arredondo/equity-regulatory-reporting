using MediatR;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.UpdateBoardMember;

public record UpdateBoardMemberCommand(
    Guid Id,
    Guid CompanyId,
    Guid MemberId,
    Guid PrimaryPositionId,
    Guid? SecondaryPositionId) : IRequest;
