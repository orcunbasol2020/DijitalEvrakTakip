using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Yayınlanan gelen evrakların EYP paketi üretilip Atlas'a aktarıldığı kuyruk.
/// Kuyruk durumu IncomingDocument.SubmissionStatus (PublishStatusEnum) alanındadır:
/// Kuyrukta → Aktarılıyor → Yayınlandı; geçici hatada Kuyrukta'ya döner, kalıcı hatada Hatalı olur.
/// </summary>
public interface IAtlasTransferService
{
    /// <summary>
    /// Kuyruktan bir parti evrak alıp aktarır. <paramref name="ignoreEnabledFlag"/> true ise
    /// otomatik aktarım ayarı kapalı olsa da çalışır.
    /// </summary>
    Task<AtlasTransferRunResultDto> RunAsync(bool ignoreEnabledFlag, CancellationToken cancellationToken);

    /// <summary>Arka plan servisinin iki tur arasında bekleyeceği süre.</summary>
    Task<TimeSpan> GetIntervalAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Evrakı aktarım kuyruğuna alır: yayınlanabilir mi diye kontrol eder (eksik bilgide hata fırlatır),
    /// önceki hatalı paketi kaldırır ve SubmissionStatus'u Kuyrukta yapar. SaveChanges çağırmaz.
    /// </summary>
    Task EnqueueAsync(IncomingDocument document, CancellationToken cancellationToken);

    /// <summary>Hatalı aktarımı evrakın güncel bilgileriyle yeniden kuyruğa alır.</summary>
    Task RetryAsync(Guid documentId, string userId, CancellationToken cancellationToken);

    Task<AtlasTransferStatsDto> GetStatsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<AtlasTransferFailedDocumentDto>> GetFailedAsync(CancellationToken cancellationToken);

    /// <summary>Servisin ayarları, son/sonraki turu, ön koşul kontrolleri ve sayılar.</summary>
    Task<AtlasTransferStatusDto> GetStatusAsync(CancellationToken cancellationToken);

    /// <summary>Kuyrukta bekleyen ve o anda aktarılan evraklar, sıraya alınma sırasıyla.</summary>
    Task<IReadOnlyList<AtlasTransferQueueItemDto>> GetQueueAsync(CancellationToken cancellationToken);

    /// <summary>Tüm evraklardaki son gönderim denemeleri, en yenisi önce.</summary>
    Task<IReadOnlyList<AtlasTransferAttemptDto>> GetRecentAttemptsAsync(int limit, CancellationToken cancellationToken);

    /// <summary>Evrakın aktarım durumu, paketleri ve denemeleri. Evrak yoksa hata fırlatır.</summary>
    Task<AtlasTransferDocumentDto> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken);

    /// <summary>
    /// Evrakın EYP paket dosyası. <paramref name="packageId"/> verilmezse güncel paket döner.
    /// Paket üretilmemişse veya dosyası yoksa hata fırlatır.
    /// </summary>
    Task<AtlasTransferPackageFileDto> GetPackageFileAsync(Guid documentId, Guid? packageId, CancellationToken cancellationToken);
}
