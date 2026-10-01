using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories
{
    public sealed class EnvelopeRepository : Repository<Envelope, AppDbContext>, IEnvelopeRepository
    {
        public EnvelopeRepository(AppDbContext context) : base(context)
        {
        }
    }
}