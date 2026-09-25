using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class UserLoginLogRepository : Repository<UserLoginLog, AppDbContext>, IUserLoginLogRepository
{
    public UserLoginLogRepository(AppDbContext context) : base(context)
    {
    }
}
