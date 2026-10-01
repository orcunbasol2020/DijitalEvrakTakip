using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class OutgoingDocumentDistributionRepository
    : Repository<OutgoingDocumentDistribution, AppDbContext>,
      IOutgoingDocumentDistributionRepository
{
    public OutgoingDocumentDistributionRepository(AppDbContext context)
        : base(context)
    {
    }
}
