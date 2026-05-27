using equity_regulatory_reporting.Domain.Common;

namespace equity_regulatory_reporting.Domain.Entities;

public class Participation : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public Person Company { get; set; } = null!;

    public Guid ShareholderId { get; set; }
    public Person Shareholder { get; set; } = null!;

    public decimal Percentage { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}
