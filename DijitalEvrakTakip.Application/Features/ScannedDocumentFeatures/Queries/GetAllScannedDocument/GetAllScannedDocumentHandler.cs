using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Queries.GetAllScannedDocument;

public sealed class GetAllScannedDocumentHandler
    : IRequestHandler<GetAllScannedDocumentQuery, IList<ScannedDocument>>
{
    private readonly IScannedDocumentService _scannedDocumentService;

    public GetAllScannedDocumentHandler(IScannedDocumentService scannedDocumentService)
    {
        _scannedDocumentService = scannedDocumentService;
    }

    public async Task<IList<ScannedDocument>> Handle(
        GetAllScannedDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await _scannedDocumentService
            .GetAllAsync(request, cancellationToken);

        return documents;
    }
}