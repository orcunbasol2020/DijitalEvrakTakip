using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class DocumentAssignmentConfiguration
    : IEntityTypeConfiguration<DocumentAssignment>
{
    public void Configure(EntityTypeBuilder<DocumentAssignment> builder)
    {
        builder.ToTable("DocumentAssignments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.DocumentId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(450); // string ise genelde 450 (Identity uyumlu)

        builder.Property(x => x.Lock);

        builder.Property(x => x.IsActive);

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.HasOne(x => x.IncomingDocument)
            .WithMany(x => x.DocumentAssignments)
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}