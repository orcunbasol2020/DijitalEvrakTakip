using DijitalEvrakTakip.Domain.Abstractions;
using DijitalEvrakTakip.Persistance.Configurations;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Context;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly); // proje icindeki tum configurationlar context baglanmis oldu.


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entires = ChangeTracker.Entries<Entity>();
        foreach (var entire in entires) { 
            if(entire.State == EntityState.Added)
                entire.Property(p=> p.CreatedDate).CurrentValue = DateTime.UtcNow;
        }

        foreach (var entire in entires)
        {
            if (entire.State == EntityState.Modified)
                entire.Property(p => p.UpdateDate).CurrentValue = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    //base.OnModelCreating(modelBuilder);
    //// Manuel olarak her konfigürasyonu ekliyoruz:
    //modelBuilder.ApplyConfiguration(new UserConfiguration());

}
