namespace equity_regulatory_reporting.Application.Features.BoardMembers.Dtos;

public record BoardMemberDto(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    Guid MemberId,
    string MemberName,
    Guid PrimaryPositionId,
    string PrimaryPositionName,
    Guid SecondaryPositionId,
    string SecondaryPositionName);
