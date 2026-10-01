using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class OutgoingDocumentRepository
    : Repository<OutgoingDocument, AppDbContext>,
      IOutgoingDocumentRepository
{
    public OutgoingDocumentRepository(AppDbContext context)
        : base(context)
    {
    }
}
