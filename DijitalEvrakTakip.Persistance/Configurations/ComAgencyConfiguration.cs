using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

// EGovernmentCmsCommon.dbo.ComAgency şemasıyla birebir; tablo adı ve kolon tipleri bilerek korunur
public sealed class ComAgencyConfiguration : IEntityTypeConfiguration<ComAgency>
{
    public void Configure(EntityTypeBuilder<ComAgency> builder)
    {
        builder.ToTable("ComAgency");

        builder.HasKey(x => x.AgencyId);

        // Id'ler kaynak sistemden gelir, identity değil
        builder.Property(x => x.AgencyId).ValueGeneratedNever();

        builder.Property(x => x.AgencyCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.MainCode).HasColumnType("char(2)").IsUnicode(false).IsFixedLength().HasMaxLength(2);
        builder.Property(x => x.DepartmentCode).HasColumnType("nchar(3)").IsFixedLength().HasMaxLength(3);
        builder.Property(x => x.SubAgencyCode1).HasColumnType("char(2)").IsUnicode(false).IsFixedLength().HasMaxLength(2);
        builder.Property(x => x.SubAgencyCode2).HasColumnType("char(2)").IsUnicode(false).IsFixedLength().HasMaxLength(2);
        builder.Property(x => x.SubAgencyCode3).HasColumnType("char(2)").IsUnicode(false).IsFixedLength().HasMaxLength(2);
        builder.Property(x => x.SubAgencyCode4).HasColumnType("char(2)").IsUnicode(false).IsFixedLength().HasMaxLength(2);
        builder.Property(x => x.DistributionCode1).HasColumnType("char(1)").IsUnicode(false).IsFixedLength().HasMaxLength(1);
        builder.Property(x => x.DistributionCode2).HasColumnType("char(1)").IsUnicode(false).IsFixedLength().HasMaxLength(1);
        builder.Property(x => x.DistributionCode3).HasColumnType("char(1)").IsUnicode(false).IsFixedLength().HasMaxLength(1);
        builder.Property(x => x.DistributionCode4).HasColumnType("char(1)").IsUnicode(false).IsFixedLength().HasMaxLength(1);
        builder.Property(x => x.HasSubAgency).HasDefaultValue(false);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Address).HasMaxLength(300);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Fax).HasMaxLength(150);
        builder.Property(x => x.Phone).HasMaxLength(150);
        builder.Property(x => x.TypeCode).HasColumnType("char(1)").IsUnicode(false).IsFixedLength().HasMaxLength(1);
        builder.Property(x => x.WebAddress).HasMaxLength(200);
        builder.Property(x => x.KEPAddress).HasMaxLength(200);

        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        // Varsayılanı 0 olmayan kolonlar: sentinel = varsayılan, böylece 0/false açıkça yazılabilir
        builder.Property(x => x.ECorrespondenceUsage).HasDefaultValue(1).HasSentinel(1);
        builder.Property(x => x.ECorrespondenceEndPoint).HasMaxLength(300);
        builder.Property(x => x.DataSourceId).HasDefaultValue(1).HasSentinel(1);
        builder.Property(x => x.IsCacheable).HasDefaultValue(0);
        builder.Property(x => x.IsLocked).HasDefaultValue(false);
        builder.Property(x => x.IsActive).HasDefaultValue(true).HasSentinel(true);
        builder.Property(x => x.ECorrespondenceName).HasMaxLength(300);
        builder.Property(x => x.CanSendECorrespondence).HasDefaultValue(false);
        builder.Property(x => x.ECorrespondenceType).HasDefaultValue(0);
        builder.Property(x => x.PathFlaten).HasMaxLength(300);
        builder.Property(x => x.Position).HasMaxLength(500);

        builder.Property(x => x.CorrespondenceAgencyId)
            .HasComment("Dolu ise ,yazışma burada yazan birim ile yapılabilir. Seçili birim ile direkt yazışma yapılamıyor demekki.");
        builder.Property(x => x.CorrespondenceAgencyText)
            .HasComment("Yazışma birim metni");
        builder.Property(x => x.UseCorrespondenceAgencyIdAsAgencyId)
            .HasComment("Bazı durumlarda, agencyId seçip, yazıda CorrespondenceAgency deki Name kullanılabilir.");

        builder.Property(x => x.MersisNo).HasMaxLength(100);

        builder.HasIndex(x => x.ParentAgencyId);
        builder.HasIndex(x => x.DepartmentId);
    }
}
