using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Import;

public static class BoardMemberImportRules
{
    public static IReadOnlyList<string> Check(Person company, Person member)
    {
        var errors = new List<string>();

        if (company.PersonType is not (PersonType.Legal or PersonType.LegalEntity))
            errors.Add("Company must be a Legal or LegalEntity person.");

        if (member.PersonType is not PersonType.Natural)
            errors.Add("Member must be a Natural person.");

        return errors;
    }
}
