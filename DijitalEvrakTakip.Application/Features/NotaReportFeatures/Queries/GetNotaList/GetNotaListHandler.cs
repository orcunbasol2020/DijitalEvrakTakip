using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaList;

public sealed class GetNotaListHandler : IRequestHandler<GetNotaListQuery, PagedResultDto<NotaListItemDto>>
{
    private readonly INotaReportService _notaReportService;

    public GetNotaListHandler(INotaReportService notaReportService)
    {
        _notaReportService = notaReportService;
    }

    public Task<PagedResultDto<NotaListItemDto>> Handle(GetNotaListQuery request, CancellationToken cancellationToken) =>
        _notaReportService.GetListAsync(request, cancellationToken);
}
