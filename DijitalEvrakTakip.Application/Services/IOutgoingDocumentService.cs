using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.CreateOutgoingDocument;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.UpdateOutgoingDocument;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetAllOutgoingDocument;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IOutgoingDocumentService
{
    Task<Guid> CreateAsync(CreateOutgoingDocumentCommand request, CancellationToken cancellationToken);

    Task UpdateAsync(UpdateOutgoingDocumentCommand request, CancellationToken cancellationToken);

    Task<IList<OutgoingDocument>> GetAllAsync(GetAllOutgoingDocumentQuery request, CancellationToken cancellationToken);

    Task<OutgoingDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<OutgoingDocument?> GetByQrCodeAsync(string qrCode, CancellationToken cancellationToken);
}
