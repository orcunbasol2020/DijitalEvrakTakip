using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class OutgoingDocumentShipmentRepository
    : Repository<OutgoingDocumentShipment, AppDbContext>,
      IOutgoingDocumentShipmentRepository
{
    public OutgoingDocumentShipmentRepository(AppDbContext context)
        : base(context)
    {
    }
}
