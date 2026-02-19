using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations
{
    public sealed class ExternalInstitutionConfiguration
        : IEntityTypeConfiguration<ExternalInstitution>
    {
        public void Configure(EntityTypeBuilder<ExternalInstitution> builder)
        {
            builder.ToTable("ExternalInstitutions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(x => x.Type);

            builder.Property(x => x.ParentId)
                   .IsRequired(false);

            // Self reference (Parent-Child)
            builder.HasOne(x => x.Parent)
                   .WithMany(x => x.Children)
                   .HasForeignKey(x => x.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ParentId);

            // Base entity alanları
            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            builder.Property(x => x.CreatedDate)
                   .IsRequired();

            builder.Property(x => x.UpdateDate)
                   .IsRequired(false);

            // 🔥 Soft Delete Global Filter
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
