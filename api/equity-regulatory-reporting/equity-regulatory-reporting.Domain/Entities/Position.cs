using equity_regulatory_reporting.Domain.Common;

namespace equity_regulatory_reporting.Domain.Entities;

public class Position : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string ReportCode { get; set; } = string.Empty;

    public ICollection<BoardMember> PrimaryMembers { get; set; } = [];
    public ICollection<BoardMember> SecondaryMembers { get; set; } = [];
}
