using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class DocumentAllocationRequestConfiguration
    : IEntityTypeConfiguration<DocumentAllocationRequest>
{
    public void Configure(EntityTypeBuilder<DocumentAllocationRequest> builder)
    {
        builder.ToTable("DocumentAllocationRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.IncomingDocumentId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.ToUserId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.RequestedByUserId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.RequestedAllocationStatus)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue((int)AllocationRequestStatusEnum.Beklemede);

        builder.Property(x => x.ResponseNote)
            .HasMaxLength(1000);

        builder.Property(x => x.HasDiscrepancy)
            .IsRequired();

        builder.Property(x => x.ReminderCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        // Bir evrakta aynı anda tek bekleyen talep olabilir
        builder.HasIndex(x => x.IncomingDocumentId)
            .IsUnique()
            .HasFilter("[Status] = 1 AND [IsDeleted] = 0")
            .HasDatabaseName("UX_DocumentAllocationRequests_IncomingDocumentId_Pending");

        // Onayımı bekleyenler listesi
        builder.HasIndex(x => new { x.ToUserId, x.Status, x.IsDeleted })
            .HasDatabaseName("IX_DocumentAllocationRequests_ToUserId_Status_IsDeleted");

        // Hatırlatma job'ı
        builder.HasIndex(x => new { x.Status, x.NextReminderDate })
            .HasDatabaseName("IX_DocumentAllocationRequests_Status_NextReminderDate");
    }
}
