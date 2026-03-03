using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentById;

public sealed class GetIncomingDocumentByIdHandler
    : IRequestHandler<GetIncomingDocumentByIdQuery, IncomingDocument>
{
    private readonly IIncomingDocumentRepository _documentRepository;

    public GetIncomingDocumentByIdHandler(IIncomingDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<IncomingDocument> Handle(GetIncomingDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _documentRepository.GetByExpressionAsync(
            x => x.Id == request.Id, cancellationToken);
    }
}