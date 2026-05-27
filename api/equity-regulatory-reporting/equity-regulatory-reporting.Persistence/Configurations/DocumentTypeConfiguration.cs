using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace equity_regulatory_reporting.Persistence.Configurations;

public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Abbreviation).IsRequired().HasMaxLength(20);
        builder.Property(d => d.ValidationRegex).HasMaxLength(500);
        builder.HasIndex(d => d.Abbreviation).IsUnique();
    }
}
