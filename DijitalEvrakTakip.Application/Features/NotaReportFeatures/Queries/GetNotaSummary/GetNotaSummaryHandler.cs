using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSummary;

public sealed class GetNotaSummaryHandler : IRequestHandler<GetNotaSummaryQuery, NotaSummaryDto>
{
    private readonly INotaReportService _notaReportService;

    public GetNotaSummaryHandler(INotaReportService notaReportService)
    {
        _notaReportService = notaReportService;
    }

    public Task<NotaSummaryDto> Handle(GetNotaSummaryQuery request, CancellationToken cancellationToken) =>
        _notaReportService.GetSummaryAsync(request, cancellationToken);
}
