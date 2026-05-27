using equity_regulatory_reporting.Domain.Common;

namespace equity_regulatory_reporting.Domain.Entities;

public class DocumentType : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? ValidationRegex { get; set; }

    public ICollection<DocumentTypePersonType> AllowedPersonTypes { get; set; } = [];
    public ICollection<Person> Persons { get; set; } = [];
}
