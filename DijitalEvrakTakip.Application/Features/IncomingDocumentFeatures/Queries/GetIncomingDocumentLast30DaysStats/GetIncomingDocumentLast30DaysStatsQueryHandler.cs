using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentLast30DaysStats;

public sealed class GetIncomingDocumentLast30DaysStatsQueryHandler
    : IRequestHandler<GetIncomingDocumentLast30DaysStatsQuery, IncomingDocumentLast30DaysStatsDto>
{
    private readonly IIncomingDocumentService _service;

    public GetIncomingDocumentLast30DaysStatsQueryHandler(IIncomingDocumentService service)
    {
        _service = service;
    }

    public async Task<IncomingDocumentLast30DaysStatsDto> Handle(
        GetIncomingDocumentLast30DaysStatsQuery request,
        CancellationToken cancellationToken)
    {
        return await _service.GetLast30DaysStatsAsync(cancellationToken);
    }
}