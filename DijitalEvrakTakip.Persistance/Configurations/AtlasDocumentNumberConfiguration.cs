using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class AtlasDocumentNumberConfiguration
    : IEntityTypeConfiguration<AtlasDocumentNumber>
{
    public void Configure(EntityTypeBuilder<AtlasDocumentNumber> builder)
    {
        builder.ToTable("AtlasDocumentNumbers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.QrCode)
            .IsRequired()
            .HasMaxLength(50);

        // Aynı numara havuza iki kez girmesin
        builder.HasIndex(x => x.QrCode)
            .IsUnique();

        builder.Property(x => x.Status)
            .IsRequired();

        // Boştaki numaraların sırayla ayrılması (Status + CreatedDate) için
        builder.HasIndex(x => new { x.Status, x.CreatedDate });

        builder.Property(x => x.AtlasReferenceId)
            .HasMaxLength(100);

        builder.Property(x => x.ReservedAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.UsedAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.CancelledAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.CancelReason)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.UpdateDate)
            .HasColumnType("datetime2");

        builder.HasOne(x => x.IncomingDocument)
            .WithMany()
            .HasForeignKey(x => x.IncomingDocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
