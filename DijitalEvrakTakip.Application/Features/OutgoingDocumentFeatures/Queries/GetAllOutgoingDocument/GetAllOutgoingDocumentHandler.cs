using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetAllOutgoingDocument;

public sealed class GetAllOutgoingDocumentHandler
    : IRequestHandler<GetAllOutgoingDocumentQuery, IList<OutgoingDocument>>
{
    private readonly IOutgoingDocumentService _outgoingDocumentService;

    public GetAllOutgoingDocumentHandler(IOutgoingDocumentService outgoingDocumentService)
    {
        _outgoingDocumentService = outgoingDocumentService;
    }

    public async Task<IList<OutgoingDocument>> Handle(
        GetAllOutgoingDocumentQuery request,
        CancellationToken cancellationToken)
    {
        return await _outgoingDocumentService.GetAllAsync(request, cancellationToken);
    }
}
