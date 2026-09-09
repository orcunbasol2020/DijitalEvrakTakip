using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class DocumentAllocationConfiguration
    : IEntityTypeConfiguration<DocumentAllocation>
{
    public void Configure(EntityTypeBuilder<DocumentAllocation> builder)
    {
        builder.ToTable("DocumentAllocations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Source)
            .IsRequired()
            .HasDefaultValue((int)AllocationSourceEnum.EvrakTakip);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        // ✅ DOĞRU FK TANIMI
        builder.HasOne(x => x.IncomingDocument)
            .WithMany(x => x.DocumentAllocations)
            .HasForeignKey(x => x.IncomingDocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}