using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetDocumentsByStatus;

public sealed class GetDocumentsByStatusHandler
    : IRequestHandler<GetDocumentsByStatusQuery, IList<IncomingDocument>>
{
    private readonly IIncomingDocumentService _incomingDocumentService;

    public GetDocumentsByStatusHandler(IIncomingDocumentService incomingDocumentService)
    {
        _incomingDocumentService = incomingDocumentService;
    }

    public async Task<IList<IncomingDocument>> Handle(GetDocumentsByStatusQuery request, CancellationToken cancellationToken)
    {
        return await _incomingDocumentService.GetAllByStatusAsync(request, cancellationToken);
    }
}
