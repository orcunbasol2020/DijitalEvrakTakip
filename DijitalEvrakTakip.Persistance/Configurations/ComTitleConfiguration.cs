using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

// EGovernmentCmsCommon.dbo.ComTitle gelene kadar geçici şema; gelince kolonlar ona hizalanır.
// ComAuthorizationUser.TitleId'den FK yok, Com tabloları kaynaktan yeniden doldurulabilir.
public sealed class ComTitleConfiguration : IEntityTypeConfiguration<ComTitle>
{
    public void Configure(EntityTypeBuilder<ComTitle> builder)
    {
        builder.ToTable("ComTitle");

        builder.HasKey(x => x.TitleId);

        // Id'ler kaynak sistemden gelir, identity değil
        builder.Property(x => x.TitleId).ValueGeneratedNever();

        builder.Property(x => x.DefaultName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.DefaultShortName).HasMaxLength(255);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
    }
}
