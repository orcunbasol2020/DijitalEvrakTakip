using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.UpdateScannedDocument;
using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Queries.GetAllScannedDocument;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IScannedDocumentService
{
    Task CreateAsync(
        ScannedDocument scannedDocument,
        CancellationToken cancellationToken);

    Task<IList<ScannedDocument>> GetAllAsync(
        GetAllScannedDocumentQuery request,
        CancellationToken cancellationToken);

    // Yeni UpdateDocumentNumberAsync metodunu ekliyoruz
    Task UpdateDocumentNumberAsync(
        UpdateScannedDocumentCommand request,
        CancellationToken cancellationToken);
}