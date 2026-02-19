using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class DocumentTransactionRepository
    : Repository<DocumentTransaction, AppDbContext>,
      IDocumentTransactionRepository
{
    public DocumentTransactionRepository(AppDbContext context)
        : base(context)
    {
    }
}
