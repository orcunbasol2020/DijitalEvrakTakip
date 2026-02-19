using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;

public sealed class GetAllIncomingDocumentHandler : IRequestHandler<GetAllIncomingDocumentQuery, IList<IncomingDocument>>
{
    private readonly IIncomingDocumentService _incomingDocumentService;

    public GetAllIncomingDocumentHandler(IIncomingDocumentService incomingDocumentService)
    {
        _incomingDocumentService = incomingDocumentService;
    }

    public async Task<IList<IncomingDocument>> Handle(GetAllIncomingDocumentQuery request, CancellationToken cancellationToken)
    {
        // Servis üzerinden IncomingDocument tablosunu filtreli çekiyoruz
        var documents = await _incomingDocumentService.GetAllAsync(request, cancellationToken);
        return documents;
    }

}
