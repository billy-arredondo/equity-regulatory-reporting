using MediatR;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.CreateBoardMember;

public record CreateBoardMemberCommand(
    Guid CompanyId,
    Guid MemberId,
    Guid PrimaryPositionId,
    Guid? SecondaryPositionId) : IRequest<Guid>;
