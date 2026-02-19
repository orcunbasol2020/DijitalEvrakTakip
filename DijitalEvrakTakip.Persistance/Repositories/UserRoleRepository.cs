using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class UserRoleRepository : Repository<UserRole, AppDbContext>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }
}

