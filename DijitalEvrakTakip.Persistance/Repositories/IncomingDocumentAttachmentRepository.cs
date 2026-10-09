using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class IncomingDocumentAttachmentRepository
    : Repository<IncomingDocumentAttachment, AppDbContext>,
      IIncomingDocumentAttachmentRepository
{
    public IncomingDocumentAttachmentRepository(AppDbContext context)
        : base(context)
    {
    }
}
