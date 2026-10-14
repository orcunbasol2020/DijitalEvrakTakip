using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations
{
    // ComDepartment üzerine view (migration'da oluşturulur), yalnızca okunur.
    // View'a bağlı ilişkiler için veritabanında FK oluşmaz; Com tabloları kaynaktan yeniden doldurulabilir.
    public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToView("vw_Departments");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Parent)
                   .WithMany(x => x.Children)
                   .HasForeignKey(x => x.ParentId);
        }
    }
}
