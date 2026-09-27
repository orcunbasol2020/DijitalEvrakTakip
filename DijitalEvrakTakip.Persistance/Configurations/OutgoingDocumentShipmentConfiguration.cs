using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class OutgoingDocumentShipmentConfiguration
    : IEntityTypeConfiguration<OutgoingDocumentShipment>
{
    public void Configure(EntityTypeBuilder<OutgoingDocumentShipment> builder)
    {
        builder.ToTable("OutgoingDocumentShipments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CargoCompany)
            .IsRequired();

        builder.Property(x => x.TrackingNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SentDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.SentUserId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(x => x.ExternalInstitutionId)
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.Property(x => x.RecipientName)
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue((int)ShipmentStatusEnum.Shipped);

        builder.Property(x => x.DeliveredDate)
            .HasColumnType("datetime2");

        builder.Property(x => x.Cost)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.ExternalInstitution)
            .WithMany()
            .HasForeignKey(x => x.ExternalInstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Takip numarası ile arama için
        builder.HasIndex(x => x.TrackingNumber);
    }
}
