using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace equity_regulatory_reporting.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Code).IsRequired().HasMaxLength(6);
        builder.Property(l => l.Department).IsRequired().HasMaxLength(200);
        builder.Property(l => l.Province).IsRequired().HasMaxLength(200);
        builder.Property(l => l.District).IsRequired().HasMaxLength(200);
        builder.HasIndex(l => l.Code).IsUnique();
    }
}
