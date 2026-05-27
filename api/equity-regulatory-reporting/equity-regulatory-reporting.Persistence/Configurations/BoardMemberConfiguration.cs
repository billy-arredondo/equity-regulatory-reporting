using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace equity_regulatory_reporting.Persistence.Configurations;

public class BoardMemberConfiguration : IEntityTypeConfiguration<BoardMember>
{
    public void Configure(EntityTypeBuilder<BoardMember> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasOne(b => b.Company)
            .WithMany()
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Member)
            .WithMany()
            .HasForeignKey(b => b.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.PrimaryPosition)
            .WithMany(p => p.PrimaryMembers)
            .HasForeignKey(b => b.PrimaryPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.SecondaryPosition)
            .WithMany(p => p.SecondaryMembers)
            .HasForeignKey(b => b.SecondaryPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.CompanyId);
        builder.HasIndex(b => b.MemberId);
    }
}
