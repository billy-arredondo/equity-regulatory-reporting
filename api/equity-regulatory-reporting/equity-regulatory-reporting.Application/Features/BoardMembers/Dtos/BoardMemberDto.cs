using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Dtos;

public record BoardMemberDto(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    PersonType CompanyPersonType,
    Guid MemberId,
    string MemberName,
    Guid PrimaryPositionId,
    string PrimaryPositionName,
    Guid SecondaryPositionId,
    string SecondaryPositionName);
