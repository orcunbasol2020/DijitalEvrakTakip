using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class OutgoingDocumentConfiguration
    : IEntityTypeConfiguration<OutgoingDocument>
{
    public void Configure(EntityTypeBuilder<OutgoingDocument> builder)
    {
        builder.ToTable("OutgoingDocuments");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        // String Fields
        builder.Property(x => x.QrCode)
            .HasMaxLength(50);

        builder.Property(x => x.OriginalDocumentNumber)
            .HasMaxLength(50);

        builder.Property(x => x.SecurityDegree)
            .HasMaxLength(50);

        builder.Property(x => x.Subject)
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.Content_Ocr)
            .HasColumnType("nvarchar(max)");

        // Numeric / Enum Fields
        builder.Property(x => x.Type);

        builder.Property(x => x.LanguageId);

        builder.Property(x => x.Status);

        builder.Property(x => x.PageCount);

        builder.Property(x => x.Source)
            .IsRequired()
            .HasDefaultValue((int)AllocationSourceEnum.EvrakTakip);

        // Boolean Fields
        builder.Property(x => x.ElectronicCopy);

        builder.Property(x => x.EbysTransfer);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        // Date Fields
        builder.Property(x => x.DocumentDate)
            .HasColumnType("datetime2");

        builder.Property(x => x.CreatedDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.UpdateDate)
            .HasColumnType("datetime2");

        // -----------------------------
        // Foreign Keys (NULLABLE)
        // -----------------------------

        builder.Property(x => x.DepartmentId)
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.Property(x => x.ExternalInstitutonId)
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExternalInstitution)
            .WithMany()
            .HasForeignKey(x => x.ExternalInstitutonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedUserId)
            .HasMaxLength(450);
    }
}
