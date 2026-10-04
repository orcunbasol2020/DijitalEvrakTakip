using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

// EGovernmentCmsCommon.dbo.ComDepartment şemasıyla birebir.
// Kaynaktaki ComCountry / ComDepartmentType FK'leri bu veritabanında o tablolar olmadığı için yok.
public sealed class ComDepartmentConfiguration : IEntityTypeConfiguration<ComDepartment>
{
    public void Configure(EntityTypeBuilder<ComDepartment> builder)
    {
        builder.ToTable("ComDepartment");

        builder.HasKey(x => x.DepartmentId);

        // Id'ler kaynak sistemden gelir, identity değil
        builder.Property(x => x.DepartmentId).ValueGeneratedNever();

        builder.Property(x => x.Code).HasColumnType("char(4)").IsUnicode(false).IsFixedLength().HasMaxLength(4);
        builder.Property(x => x.DefaultName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.DefaultShortName).HasMaxLength(255);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.Property(x => x.CamLink).HasMaxLength(500);
        builder.Property(x => x.Email).HasMaxLength(200);

        builder.HasIndex(x => x.ParentDepartmentId);
    }
}
