using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class RoleRepository : Repository<Role, AppDbContext>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }
}
