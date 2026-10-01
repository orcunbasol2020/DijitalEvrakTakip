using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class AtlasZimmetChangeRepository
    : Repository<AtlasZimmetChange, AppDbContext>,
      IAtlasZimmetChangeRepository
{
    public AtlasZimmetChangeRepository(AppDbContext context)
        : base(context)
    {
    }
}
