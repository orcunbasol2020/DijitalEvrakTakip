using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class UserRepository : Repository<User, AppDbContext>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
}
