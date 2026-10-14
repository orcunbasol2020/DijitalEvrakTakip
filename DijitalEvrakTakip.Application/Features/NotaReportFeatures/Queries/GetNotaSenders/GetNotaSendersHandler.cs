using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSenders;

public sealed class GetNotaSendersHandler : IRequestHandler<GetNotaSendersQuery, IList<NotaSenderDto>>
{
    private readonly INotaReportService _notaReportService;

    public GetNotaSendersHandler(INotaReportService notaReportService)
    {
        _notaReportService = notaReportService;
    }

    public Task<IList<NotaSenderDto>> Handle(GetNotaSendersQuery request, CancellationToken cancellationToken) =>
        _notaReportService.GetSendersAsync(request, cancellationToken);
}
