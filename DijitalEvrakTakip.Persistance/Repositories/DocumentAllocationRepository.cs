using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class DocumentAllocationRepository
    : Repository<DocumentAllocation, AppDbContext>,
      IDocumentAllocationRepository
{
    public DocumentAllocationRepository(AppDbContext context)
        : base(context)
    {
    }
}
