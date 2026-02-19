using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetDocumentByNumber;

public sealed class GetDocumentByNumberHandler
    : IRequestHandler<GetDocumentByNumberQuery, Document>
{
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentByNumberHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<Document> Handle(GetDocumentByNumberQuery request, CancellationToken cancellationToken)
    {
        return await _documentRepository.GetByExpressionAsync(
            x => x.BelgeId == request.documentNumber, cancellationToken);
    }
}
