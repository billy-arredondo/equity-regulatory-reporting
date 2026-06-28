using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace equity_regulatory_reporting.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(300);
        builder.Property(p => p.PersonType).HasConversion<int>();
        builder.Property(p => p.Ciiu).HasMaxLength(10);
        builder.Property(p => p.Address).HasMaxLength(500);
        builder.Property(p => p.DocumentNumber).HasMaxLength(50);
        builder.Property(p => p.EntityCode).HasMaxLength(50);
        builder.Property(p => p.LegacyId);
        builder.Property(p => p.RepresentativeDescription).HasMaxLength(300);
        builder.HasIndex(p => p.LegacyId).IsUnique().HasFilter("legacy_id IS NOT NULL");

        builder.HasOne(p => p.DocumentType)
            .WithMany(d => d.Persons)
            .HasForeignKey(p => p.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Country)
            .WithMany(c => c.Persons)
            .HasForeignKey(p => p.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Representative)
            .WithMany()
            .HasForeignKey(p => p.RepresentativeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Location)
            .WithMany(l => l.Persons)
            .HasForeignKey(p => p.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.CountryId);
        builder.HasIndex(p => p.DocumentTypeId);
        builder.HasIndex(p => p.LocationId);
    }
}
