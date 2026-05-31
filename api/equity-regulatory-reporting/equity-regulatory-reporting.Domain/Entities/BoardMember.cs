using equity_regulatory_reporting.Domain.Common;

namespace equity_regulatory_reporting.Domain.Entities;

public class BoardMember : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public Person Company { get; set; } = null!;

    public Guid MemberId { get; set; }
    public Person Member { get; set; } = null!;

    public Guid PrimaryPositionId { get; set; }
    public Position PrimaryPosition { get; set; } = null!;

    public Guid SecondaryPositionId { get; set; }
    public Position SecondaryPosition { get; set; } = null!;
}
