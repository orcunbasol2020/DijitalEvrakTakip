using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class IncomingDocumentConfiguration
    : IEntityTypeConfiguration<IncomingDocument>
{
    public void Configure(EntityTypeBuilder<IncomingDocument> builder)
    {
        builder.ToTable("IncomingDocuments");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        // String Fields
        builder.Property(x => x.OrginalNo)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(x => x.QrCode)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(x => x.DocumentName)
            .HasMaxLength(200)
            .IsUnicode(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(500)
            .IsUnicode(false);

        builder.Property(x => x.Subject)
            .HasMaxLength(200);

        builder.Property(x => x.Content_Ocr)
            .HasColumnType("nvarchar(max)");

        // Numeric / Enum Fields
        builder.Property(x => x.SecurityDegree);

        builder.Property(x => x.DocumentTypeId);

        builder.Property(x => x.LanguageId);

        builder.Property(x => x.Status);

        builder.Property(x => x.SubmissionStatus);

        builder.Property(x => x.OcrStatus);

        builder.Property(x => x.PageCount);

        // Boolean Fields
        builder.Property(x => x.ElectronicCopy);

        builder.Property(x => x.Release);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        // Date Fields
        builder.Property(x => x.DocumentDate)
            .HasColumnType("datetime2");

        builder.Property(x => x.ReleaseDate)
            .HasColumnType("datetime2");

        builder.Property(x => x.CreatedDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.UpdateDate)
            .HasColumnType("datetime2");

        // -----------------------------
        // Foreign Keys (NULLABLE)
        // -----------------------------

        builder.Property(x => x.ExternalInstitutionId)
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.Property(x => x.DepartmentId)
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.HasOne(x => x.ExternalInstitution)
            .WithMany()
            .HasForeignKey(x => x.ExternalInstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // User (Eğer Identity kullanıyorsan max 450 önerilir)
        builder.Property(x => x.UserId)
            .HasMaxLength(450);
    }
}
