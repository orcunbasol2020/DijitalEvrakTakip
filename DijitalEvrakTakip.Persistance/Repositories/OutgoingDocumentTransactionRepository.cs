using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class OutgoingDocumentTransactionRepository
    : Repository<OutgoingDocumentTransaction, AppDbContext>,
      IOutgoingDocumentTransactionRepository
{
    public OutgoingDocumentTransactionRepository(AppDbContext context)
        : base(context)
    {
    }
}
