using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Features.Participations.Import;

public static class ParticipationImportRules
{
    public static IReadOnlyList<string> Check(Person company)
    {
        var errors = new List<string>();

        if (company.PersonType is not (PersonType.Legal or PersonType.LegalEntity))
            errors.Add("Company must be a Legal or LegalEntity person.");

        return errors;
    }
}
