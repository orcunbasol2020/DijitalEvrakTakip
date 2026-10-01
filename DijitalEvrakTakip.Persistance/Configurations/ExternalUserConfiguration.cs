using System;
using System.Collections.Generic;
using System.Linq;
using DijitalEvrakTakip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DijitalEvrakTakip.Persistance.Configurations;

public sealed class ExternalUserConfiguration : IEntityTypeConfiguration<ExternalUser>
{
    public void Configure(EntityTypeBuilder<ExternalUser> builder)
    {
        builder.ToTable("ExternalUser");
        builder.HasKey(x => x.Id);


    }
}
