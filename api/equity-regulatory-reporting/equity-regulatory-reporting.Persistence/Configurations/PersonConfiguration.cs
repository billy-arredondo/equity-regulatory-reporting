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
        builder.Property(p => p.Ciiu).IsRequired().IsFixedLength().HasMaxLength(10);
        builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
        builder.Property(p => p.DocumentNumber).IsRequired().HasMaxLength(50);
        builder.Property(p => p.EntityCode).HasMaxLength(50);
        builder.Property(p => p.InternalLocation).IsRequired().HasMaxLength(500);

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

        builder.HasIndex(p => p.CountryId);
        builder.HasIndex(p => p.DocumentTypeId);
    }
}
