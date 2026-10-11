using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations
{
    // ComAuthorizationUser + ComAgency üzerine view (migration'da oluşturulur), yalnızca okunur.
    // View'a bağlı ilişkiler için veritabanında FK oluşmaz; Com tabloları kaynaktan yeniden doldurulabilir.
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToView("vw_Users");
            builder.HasKey(x => x.Id);

            builder
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId);
        }
    }
}
