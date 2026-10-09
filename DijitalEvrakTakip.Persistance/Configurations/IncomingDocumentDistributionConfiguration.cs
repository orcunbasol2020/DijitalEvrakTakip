using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class IncomingDocumentDistributionConfiguration
    : IEntityTypeConfiguration<IncomingDocumentDistribution>
{
    public void Configure(EntityTypeBuilder<IncomingDocumentDistribution> builder)
    {
        builder.ToTable("IncomingDocumentDistributions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IncomingDocumentId)
            .IsRequired();

        builder.Property(x => x.DepartmentId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.IncomingDocument)
            .WithMany(x => x.Distributions)
            .HasForeignKey(x => x.IncomingDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.IncomingDocumentId);
        builder.HasIndex(x => x.DepartmentId);
    }
}
