using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class AppSettingRepository : Repository<AppSetting, AppDbContext>, IAppSettingRepository
{
    public AppSettingRepository(AppDbContext context) : base(context)
    {
    }
}
