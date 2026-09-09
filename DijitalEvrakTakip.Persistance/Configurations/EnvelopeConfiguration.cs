using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class EnvelopeConfiguration : IEntityTypeConfiguration<Envelope>
{
    public void Configure(EntityTypeBuilder<Envelope> builder)
    {
        builder.ToTable("Envelopes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EnvelopeNo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired();


        builder.HasMany(x => x.EnvelopeDocuments)
            .WithOne(x => x.Envelope)
            .HasForeignKey(x => x.EnvelopeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}