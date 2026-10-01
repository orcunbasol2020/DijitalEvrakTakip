using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class OutgoingDocumentAllocationRepository
    : Repository<OutgoingDocumentAllocation, AppDbContext>,
      IOutgoingDocumentAllocationRepository
{
    public OutgoingDocumentAllocationRepository(AppDbContext context)
        : base(context)
    {
    }
}
