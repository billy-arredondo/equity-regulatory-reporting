using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace equity_regulatory_reporting.Persistence.Configurations;

public class DocumentTypePersonTypeConfiguration : IEntityTypeConfiguration<DocumentTypePersonType>
{
    public void Configure(EntityTypeBuilder<DocumentTypePersonType> builder)
    {
        builder.HasKey(m => new { m.DocumentTypeId, m.PersonType });

        builder.Property(m => m.PersonType)
            .HasConversion<int>();

        builder.HasOne(m => m.DocumentType)
            .WithMany(d => d.AllowedPersonTypes)
            .HasForeignKey(m => m.DocumentTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
