using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations
{
    // ComAgency üzerine view (migration'da oluşturulur), yalnızca okunur.
    // View'a bağlı ilişkiler için veritabanında FK oluşmaz; Com tabloları kaynaktan yeniden doldurulabilir.
    // Silinmiş kurumlar için global süzgeç yok: eski evraklarda gönderen adı görünmeli, listeler kendisi süzer.
    public sealed class ExternalInstitutionConfiguration
        : IEntityTypeConfiguration<ExternalInstitution>
    {
        public void Configure(EntityTypeBuilder<ExternalInstitution> builder)
        {
            builder.ToView("vw_ExternalInstitutions");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Parent)
                   .WithMany(x => x.Children)
                   .HasForeignKey(x => x.ParentId);
        }
    }
}
