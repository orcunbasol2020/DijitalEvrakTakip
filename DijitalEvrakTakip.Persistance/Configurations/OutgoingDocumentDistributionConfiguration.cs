using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class OutgoingDocumentDistributionConfiguration
    : IEntityTypeConfiguration<OutgoingDocumentDistribution>
{
    public void Configure(EntityTypeBuilder<OutgoingDocumentDistribution> builder)
    {
        builder.ToTable("OutgoingDocumentDistributions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OutgoingDocumentId)
            .IsRequired();

        builder.Property(x => x.DepartmentId)
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.Property(x => x.ExternalInstitutionId)
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.Property(x => x.DeliveryMethod);

        builder.Property(x => x.SentDate)
            .HasColumnType("datetime2");

        builder.Property(x => x.DeliveryDate)
            .HasColumnType("datetime2");

        builder.Property(x => x.ShipmentId)
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.OutgoingDocument)
            .WithMany(x => x.Distributions)
            .HasForeignKey(x => x.OutgoingDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExternalInstitution)
            .WithMany()
            .HasForeignKey(x => x.ExternalInstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.Distributions)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.OutgoingDocumentId);
        builder.HasIndex(x => x.ShipmentId);
    }
}
