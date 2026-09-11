using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class OutgoingDocumentTransactionConfiguration
    : IEntityTypeConfiguration<OutgoingDocumentTransaction>
{
    public void Configure(EntityTypeBuilder<OutgoingDocumentTransaction> builder)
    {
        builder.ToTable("OutgoingDocumentTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Type);

        builder.Property(x => x.CargoPostNumber)
            .HasMaxLength(100);

        builder.Property(x => x.UserId)
            .HasMaxLength(50);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.UpdateDate)
            .HasColumnType("datetime2");

        builder.HasOne(x => x.OutgoingDocument)
            .WithMany()
            .HasForeignKey(x => x.OutgoingDocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
