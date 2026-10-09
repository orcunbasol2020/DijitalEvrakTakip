using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class IncomingDocumentDistributionRepository
    : Repository<IncomingDocumentDistribution, AppDbContext>,
      IIncomingDocumentDistributionRepository
{
    public IncomingDocumentDistributionRepository(AppDbContext context)
        : base(context)
    {
    }
}
