using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations
{
    public sealed class ScannedDocumentConfiguration : IEntityTypeConfiguration<ScannedDocument>
    {
        public void Configure(EntityTypeBuilder<ScannedDocument> builder)
        {
            builder.ToTable("ScannedDocument");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName)
                .HasMaxLength(200);

            builder.Property(x => x.OriginalPath)
                .HasMaxLength(200);

            builder.Property(x => x.NewPath)
                .HasMaxLength(200);

            // Base Entity alanları için istersek explicit yazabiliriz
            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();
        }
    }
}