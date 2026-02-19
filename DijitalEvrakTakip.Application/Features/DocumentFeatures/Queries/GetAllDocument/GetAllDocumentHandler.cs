using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetAllDocument;

public sealed class GetAllDocumentHandler : IRequestHandler<GetAllDocumentQuery, IList<Document>>
{
    private readonly IDocumentService _documentService;

    public GetAllDocumentHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public async Task<IList<Document>> Handle(GetAllDocumentQuery request, CancellationToken cancellationToken)
    {
        IList<Document> documents = await _documentService.GetAllAsync(request, cancellationToken);
        return documents;
    }
}
