using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Domain.Entities;

public class DocumentTypePersonType
{
    public Guid DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = null!;

    public PersonType PersonType { get; set; }
}
