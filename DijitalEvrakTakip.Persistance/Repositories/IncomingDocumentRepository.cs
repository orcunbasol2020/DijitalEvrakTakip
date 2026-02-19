using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class IncomingDocumentRepository
    : Repository<IncomingDocument, AppDbContext>,
      IIncomingDocumentRepository
{
    public IncomingDocumentRepository(AppDbContext context)
        : base(context)
    {
    }
}
