using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class EypTransferConfiguration : IEntityTypeConfiguration<EypTransfer>
{
    public void Configure(EntityTypeBuilder<EypTransfer> builder)
    {
        builder.ToTable("EypTransfers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.EypPackageId);

        builder.Property(x => x.Error)
            .HasMaxLength(2000);

        builder.Property(x => x.AtlasReferenceId)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.UpdateDate)
            .HasColumnType("datetime2");
    }
}
