using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations
{
    public sealed class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
    {
        public void Configure(EntityTypeBuilder<TransactionType> builder)
        {
            builder.ToTable("TransactionTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever(); // Guid biz oluşturuyoruz

            builder.Property(x => x.Name)
                   .HasMaxLength(100);

            builder.Property(x => x.IsActive);

            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            builder.Property(x => x.CreatedDate)
                   .IsRequired();

            builder.Property(x => x.UpdateDate);
        }
    }
}
