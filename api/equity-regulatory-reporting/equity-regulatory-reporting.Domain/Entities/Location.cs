using equity_regulatory_reporting.Domain.Common;

namespace equity_regulatory_reporting.Domain.Entities;

public class Location : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;

    public ICollection<Person> Persons { get; set; } = [];
}
