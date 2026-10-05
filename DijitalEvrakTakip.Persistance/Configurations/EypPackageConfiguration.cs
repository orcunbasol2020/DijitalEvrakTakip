using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class EypPackageConfiguration : IEntityTypeConfiguration<EypPackage>
{
    public void Configure(EntityTypeBuilder<EypPackage> builder)
    {
        builder.ToTable("EypPackages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        // Evrak başına tek aktif paket; yeniden kuyruğa alınan evrakın eski paketi silinmiş işaretlenir
        builder.HasIndex(x => x.DocumentId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(x => x.FileName)
            .HasMaxLength(200);

        builder.Property(x => x.Sha256)
            .HasMaxLength(64);

        builder.Property(x => x.LastError)
            .HasMaxLength(2000);

        builder.Property(x => x.AtlasReferenceId)
            .HasMaxLength(100);

        builder.Property(x => x.LastAttemptAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.NextAttemptAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.TransferredAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.CreatedDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.UpdateDate)
            .HasColumnType("datetime2");

        builder.HasOne(x => x.Document)
            .WithMany()
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Transfers)
            .WithOne(x => x.EypPackage)
            .HasForeignKey(x => x.EypPackageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
