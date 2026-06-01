using System.Text.RegularExpressions;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Features.Persons.Import;

public static class PersonImportRules
{
    public static IReadOnlyList<string> Check(
        DocumentType documentType,
        PersonType personType,
        string documentNumber,
        bool hasRepresentative)
    {
        var errors = new List<string>();

        if (!documentType.AllowedPersonTypes.Any(a => a.PersonType == personType))
            errors.Add($"DocumentType '{documentType.Name}' is not allowed for PersonType '{personType}'.");

        if (personType is PersonType.Legal or PersonType.LegalEntity && !hasRepresentative)
            errors.Add("A representative is required for Legal and LegalEntity persons.");

        if (personType is PersonType.Natural && hasRepresentative)
            errors.Add("Natural persons cannot have a representative.");

        if (documentType.ValidationRegex is not null
            && !Regex.IsMatch(documentNumber, documentType.ValidationRegex))
            errors.Add($"DocumentNumber does not match the required format for '{documentType.Name}'.");

        return errors;
    }
}
