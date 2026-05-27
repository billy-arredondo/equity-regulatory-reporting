using equity_regulatory_reporting.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace equity_regulatory_reporting.Persistence.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public Permission Permissions { get; set; }
}
