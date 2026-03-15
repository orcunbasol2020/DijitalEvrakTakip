using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentTodayStats;

public sealed class GetIncomingDocumentTodayStatsQueryHandler
    : IRequestHandler<GetIncomingDocumentTodayStatsQuery, IncomingDocumentTodayStatsDto>
{
    private readonly IIncomingDocumentService _service;

    public GetIncomingDocumentTodayStatsQueryHandler(IIncomingDocumentService service)
    {
        _service = service;
    }

    public async Task<IncomingDocumentTodayStatsDto> Handle(
        GetIncomingDocumentTodayStatsQuery request,
        CancellationToken cancellationToken)
    {
        return await _service.GetTodayStatsAsync(cancellationToken);
    }
}