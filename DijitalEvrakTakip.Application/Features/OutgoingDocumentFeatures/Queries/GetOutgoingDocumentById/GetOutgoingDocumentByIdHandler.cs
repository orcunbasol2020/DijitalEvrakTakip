using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentById;

public sealed class GetOutgoingDocumentByIdHandler
    : IRequestHandler<GetOutgoingDocumentByIdQuery, OutgoingDocument>
{
    private readonly IOutgoingDocumentRepository _documentRepository;

    public GetOutgoingDocumentByIdHandler(IOutgoingDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<OutgoingDocument> Handle(
        GetOutgoingDocumentByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _documentRepository.GetByExpressionAsync(
            x => x.Id == request.Id, cancellationToken);
    }
}
