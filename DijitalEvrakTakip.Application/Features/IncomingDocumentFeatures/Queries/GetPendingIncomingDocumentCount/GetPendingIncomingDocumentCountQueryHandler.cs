
using DijitalEvrakTakip.Application.Services;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetPendingIncomingDocumentCount;

public sealed class GetPendingIncomingDocumentCountQueryHandler
    : IRequestHandler<GetPendingIncomingDocumentCountQuery, int>
{
    private readonly IIncomingDocumentService _service;

    public GetPendingIncomingDocumentCountQueryHandler(IIncomingDocumentService service)
    {
        _service = service;
    }

    public async Task<int> Handle(GetPendingIncomingDocumentCountQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetPendingCountByUserIdAsync(request.UserId, cancellationToken);
    }
}