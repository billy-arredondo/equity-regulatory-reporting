using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace equity_regulatory_reporting.Persistence.Configurations;

public class ParticipationConfiguration : IEntityTypeConfiguration<Participation>
{
    public void Configure(EntityTypeBuilder<Participation> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Percentage)
            .HasColumnType("decimal(5,2)");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Participation_Percentage", "percentage BETWEEN 0 AND 100");
            t.HasCheckConstraint("CK_Participation_EffectiveDates", "effective_to IS NULL OR effective_to >= effective_from");
        });

        builder.HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Shareholder)
            .WithMany()
            .HasForeignKey(p => p.ShareholderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.CompanyId, p.ShareholderId, p.EffectiveFrom });
    }
}
