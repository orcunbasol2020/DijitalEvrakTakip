using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

// EGovernmentCmsCommon.dbo.ComTitle şemasıyla birebir; metin kolonları kaynaktaki gibi Turkish_CI_AS.
// ComAuthorizationUser.TitleId'den FK yok, Com tabloları kaynaktan yeniden doldurulabilir.
public sealed class ComTitleConfiguration : IEntityTypeConfiguration<ComTitle>
{
    public void Configure(EntityTypeBuilder<ComTitle> builder)
    {
        builder.ToTable("ComTitle");

        builder.HasKey(x => x.TitleId);

        // Id'ler kaynak sistemden gelir, identity değil
        builder.Property(x => x.TitleId).ValueGeneratedNever();

        builder.Property(x => x.DefaultName).IsRequired().HasMaxLength(180).UseCollation(SqlCollations.Turkish);
        builder.Property(x => x.DefaultShortName).HasMaxLength(30).UseCollation(SqlCollations.Turkish);
        builder.Property(x => x.Code).HasMaxLength(50).UseCollation(SqlCollations.Turkish);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
    }
}
