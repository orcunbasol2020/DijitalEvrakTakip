using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

// Kullanıcılar ve birimler view'dan (vw_Users, vw_Departments) okunduğu için FK yok; Com kopyaları tazelenebilir.
public sealed class UserDepartmentAssignmentConfiguration : IEntityTypeConfiguration<UserDepartmentAssignment>
{
    public void Configure(EntityTypeBuilder<UserDepartmentAssignment> builder)
    {
        builder.ToTable("UserDepartmentAssignments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Source).HasMaxLength(200);

        // Kullanıcı başına tek geçerli atama
        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(x => x.DepartmentId);
    }
}
