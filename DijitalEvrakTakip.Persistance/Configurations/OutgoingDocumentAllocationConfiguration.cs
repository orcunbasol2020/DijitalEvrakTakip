using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class OutgoingDocumentAllocationConfiguration
    : IEntityTypeConfiguration<OutgoingDocumentAllocation>
{
    public void Configure(EntityTypeBuilder<OutgoingDocumentAllocation> builder)
    {
        builder.ToTable("OutgoingDocumentAllocations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.OutgoingDocumentId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.UserType)
            .IsRequired()
            .HasDefaultValue((int)AllocationUserTypeEnum.Internal);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Source)
            .IsRequired()
            .HasDefaultValue((int)AllocationSourceEnum.EvrakTakip);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .IsRequired();
    }
}
