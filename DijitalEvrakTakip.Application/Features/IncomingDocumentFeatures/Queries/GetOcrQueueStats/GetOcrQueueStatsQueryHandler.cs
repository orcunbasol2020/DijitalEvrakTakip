using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetOcrQueueStats;

public sealed class GetOcrQueueStatsQueryHandler
    : IRequestHandler<GetOcrQueueStatsQuery, IncomingDocumentOcrQueueStatsDto>
{
    private readonly IIncomingDocumentService _service;

    public GetOcrQueueStatsQueryHandler(IIncomingDocumentService service)
    {
        _service = service;
    }

    public async Task<IncomingDocumentOcrQueueStatsDto> Handle(
        GetOcrQueueStatsQuery request,
        CancellationToken cancellationToken)
    {
        return await _service.GetOcrQueueStatsAsync(cancellationToken);
    }
}