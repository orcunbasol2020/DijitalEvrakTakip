using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IIncomingDocumentService
{
    Task CreateAsync(CreateIncomingDocumentCommand request, CancellationToken cancellationToken);

    Task UpdateAsync(UpdateIncomingDocumentCommand request, CancellationToken cancellationToken);

    Task<IList<IncomingDocument>> GetAllAsync(GetAllIncomingDocumentQuery request, CancellationToken cancellationToken);

    Task<IncomingDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IncomingDocument?> GetByQrCodeAsync(string qrCode, CancellationToken cancellationToken);
}
