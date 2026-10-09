using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class IncomingDocumentAttachmentConfiguration
    : IEntityTypeConfiguration<IncomingDocumentAttachment>
{
    public void Configure(EntityTypeBuilder<IncomingDocumentAttachment> builder)
    {
        builder.ToTable("IncomingDocumentAttachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IncomingDocumentId)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(260)
            .IsRequired();

        builder.Property(x => x.StoredFileName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.UploadedUserId)
            .HasMaxLength(450);

        builder.HasOne(x => x.IncomingDocument)
            .WithMany()
            .HasForeignKey(x => x.IncomingDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.IncomingDocumentId);
    }
}
