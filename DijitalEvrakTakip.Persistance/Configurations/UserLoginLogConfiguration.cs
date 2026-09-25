using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class UserLoginLogConfiguration : IEntityTypeConfiguration<UserLoginLog>
{
    public void Configure(EntityTypeBuilder<UserLoginLog> builder)
    {
        builder.ToTable("UserLoginLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(512);

        // Enum'u veritabanında okunabilir olması için string olarak sakla.
        builder.Property(x => x.FailureReason)
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.LoginDate);

        // Kullanıcı bulunamayan denemelerde UserId null kalır, bu yüzden FK opsiyonel.
        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
