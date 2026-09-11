using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class EnvelopeDocumentConfiguration : IEntityTypeConfiguration<EnvelopeDocument>
{
    public void Configure(EntityTypeBuilder<EnvelopeDocument> builder)
    {
        builder.ToTable("EnvelopeDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EnvelopeId)
            .IsRequired();

        builder.Property(x => x.QrCode)
            .HasMaxLength(200);

        builder.Property(x => x.DocumentId)
            .HasColumnType("uniqueidentifier");
    }
}