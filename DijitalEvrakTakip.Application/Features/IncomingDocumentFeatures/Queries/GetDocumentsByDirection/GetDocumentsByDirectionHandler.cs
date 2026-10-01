using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetAllDocuments;

public sealed class GetDocumentsByDirectionHandler
    : IRequestHandler<GetDocumentsByDirectionQuery, IList<IncomingDocument>>
{
    private readonly IIncomingDocumentService _incomingDocumentService;

    public GetDocumentsByDirectionHandler(IIncomingDocumentService incomingDocumentService)
    {
        _incomingDocumentService = incomingDocumentService;
    }

    public async Task<IList<IncomingDocument>> Handle(GetDocumentsByDirectionQuery request, CancellationToken cancellationToken)
    {
        return await _incomingDocumentService.GetAllByDirectionAsync(request.DocumentDirection, cancellationToken);
    }
}