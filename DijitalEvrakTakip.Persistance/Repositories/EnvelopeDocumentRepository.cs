using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories
{
    public sealed class EnvelopeDocumentRepository : Repository<EnvelopeDocument, AppDbContext>, IEnvelopeDocumentRepository
    {
        public EnvelopeDocumentRepository(AppDbContext context) : base(context)
        {
        }
    }
}