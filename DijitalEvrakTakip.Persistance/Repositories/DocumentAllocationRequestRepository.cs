using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class DocumentAllocationRequestRepository
    : Repository<DocumentAllocationRequest, AppDbContext>,
      IDocumentAllocationRequestRepository
{
    public DocumentAllocationRequestRepository(AppDbContext context)
        : base(context)
    {
    }
}
