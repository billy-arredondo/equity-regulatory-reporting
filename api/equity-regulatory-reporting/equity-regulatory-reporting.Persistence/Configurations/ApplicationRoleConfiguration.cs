using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace equity_regulatory_reporting.Persistence.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.Property(r => r.Permissions)
            .HasConversion<int>()
            .HasDefaultValue(Permission.None);
    }
}
