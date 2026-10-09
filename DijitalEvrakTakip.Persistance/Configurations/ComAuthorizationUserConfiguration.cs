using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

// EGovernmentCmsCommon.dbo.ComAuthorizationUser şemasıyla birebir; tablo adı ve kolon tipleri bilerek korunur
public sealed class ComAuthorizationUserConfiguration : IEntityTypeConfiguration<ComAuthorizationUser>
{
    public void Configure(EntityTypeBuilder<ComAuthorizationUser> builder)
    {
        builder.ToTable("ComAuthorizationUser");

        builder.HasKey(x => x.UserId);

        // Id'ler kaynak sistemden gelir
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Surname).HasMaxLength(50);
        builder.Property(x => x.NTLogonName).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.ConnectsFrom).HasMaxLength(200);
        builder.Property(x => x.RegistrationNumber).HasMaxLength(20);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.LeaveDate).HasColumnType("datetime");
        builder.Property(x => x.CreateDate).HasColumnType("datetime");
        builder.Property(x => x.ModifyDate).HasColumnType("datetime");
        builder.Property(x => x.AuthorizedDate).HasColumnType("datetime");
        builder.Property(x => x.Password).HasMaxLength(96);
        builder.Property(x => x.SecureMobileNumber).HasMaxLength(13);
        builder.Property(x => x.SecureMobileDynamicCode).HasMaxLength(6);
        builder.Property(x => x.InfoNote).HasMaxLength(1000);
        builder.Property(x => x.StaffId).HasMaxLength(50);

        builder.HasIndex(x => x.AgencyId);
        builder.HasIndex(x => x.PersonId);
        builder.HasIndex(x => x.NTLogonName);
    }
}
