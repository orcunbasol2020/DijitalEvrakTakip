using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories
{
    public sealed class ScannedDocumentRepository
        : Repository<ScannedDocument, AppDbContext>, IScannedDocumentRepository
    {
        public ScannedDocumentRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}