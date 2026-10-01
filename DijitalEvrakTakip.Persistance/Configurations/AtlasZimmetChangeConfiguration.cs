using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class AtlasZimmetChangeConfiguration
    : IEntityTypeConfiguration<AtlasZimmetChange>
{
    public void Configure(EntityTypeBuilder<AtlasZimmetChange> builder)
    {
        builder.ToTable("AtlasZimmetChanges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.AtlasZimmetId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.AtlasZimmetId)
            .IsUnique();

        builder.Property(x => x.AtlasDocumentNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.QrCode)
            .HasMaxLength(50);

        builder.Property(x => x.FromUserSicilNo)
            .HasMaxLength(50);

        builder.Property(x => x.FromUserName)
            .HasMaxLength(200);

        builder.Property(x => x.FromUnitName)
            .HasMaxLength(200);

        builder.Property(x => x.ToUserSicilNo)
            .HasMaxLength(50);

        builder.Property(x => x.ToUserName)
            .HasMaxLength(200);

        builder.Property(x => x.ToUnitName)
            .HasMaxLength(200);

        builder.Property(x => x.ZimmetTuru)
            .HasMaxLength(100);

        builder.Property(x => x.ZimmetTarihi)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.RawPayload)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(x => x.ProcessedDate)
            .HasColumnType("datetime2");

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
