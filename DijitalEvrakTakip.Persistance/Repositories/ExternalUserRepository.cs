using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class ExternalUserRepository : Repository<ExternalUser, AppDbContext>, IExternalUserRepository
{
    public ExternalUserRepository(AppDbContext context) : base(context) { }
}