using DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaList;
using DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSenders;
using DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSummary;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Nota Raporları ekranı: gelen notaların (misyondan gelen her evrak, türü ne olursa olsun) misyon ve alıcı birim bazında özetleri.
/// Ay / yıl Türkiye saatine göre hesaplanır.
/// </summary>
public interface INotaReportService
{
    Task<IList<NotaSenderDto>> GetSendersAsync(GetNotaSendersQuery request, CancellationToken cancellationToken);

    Task<NotaSummaryDto> GetSummaryAsync(GetNotaSummaryQuery request, CancellationToken cancellationToken);

    Task<PagedResultDto<NotaListItemDto>> GetListAsync(GetNotaListQuery request, CancellationToken cancellationToken);
}
