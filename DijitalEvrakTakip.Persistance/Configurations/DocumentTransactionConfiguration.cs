using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class DocumentTransactionConfiguration
    : IEntityTypeConfiguration<DocumentTransaction>
{
    public void Configure(EntityTypeBuilder<DocumentTransaction> builder)
    {
        builder.ToTable("DocumentTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.DocumentId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.TransactionType)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .IsRequired();
    }
}
