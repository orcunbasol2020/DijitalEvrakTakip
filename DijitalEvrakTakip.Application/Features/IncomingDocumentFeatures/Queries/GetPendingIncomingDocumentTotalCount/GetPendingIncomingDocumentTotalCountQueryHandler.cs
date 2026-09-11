using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetPendingIncomingDocumentCount;

public sealed class GetPendingIncomingDocumentTotalCountQueryHandler
    : IRequestHandler<GetPendingIncomingDocumentTotalCountQuery, IncomingDocumentPendingScanStatsDto>
{
    private readonly IIncomingDocumentService _service;

    public GetPendingIncomingDocumentTotalCountQueryHandler(IIncomingDocumentService service)
    {
        _service = service;
    }

    public async Task<IncomingDocumentPendingScanStatsDto> Handle(
        GetPendingIncomingDocumentTotalCountQuery request,
        CancellationToken cancellationToken)
    {
        return await _service.GetPendingScanStatsAsync(cancellationToken);
    }
}