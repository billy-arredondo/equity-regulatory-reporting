using equity_regulatory_reporting.Domain.Common;
using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Domain.Entities;

public class Person : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public PersonType PersonType { get; set; }
    public string Ciiu { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public Guid DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = null!;

    public string DocumentNumber { get; set; } = string.Empty;
    public string? EntityCode { get; set; }

    public Guid? RepresentativeId { get; set; }
    public Person? Representative { get; set; }

    public bool ReportFlag { get; set; }

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public string InternalLocation { get; set; } = string.Empty;
}
