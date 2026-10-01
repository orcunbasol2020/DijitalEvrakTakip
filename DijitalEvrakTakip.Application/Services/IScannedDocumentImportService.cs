using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// AppSettings'teki tarama klasöründeki PDF'leri gelen evrak klasörüne taşır
/// ve her biri için eşleştirme bekleyen bir ScannedDocument kaydı oluşturur.
/// </summary>
public interface IScannedDocumentImportService
{
    /// <param name="ignoreEnabledFlag">
    /// true ise ScanImportEnabled kapalı olsa da çalışır (elle tetikleme için).
    /// </param>
    Task<ScannedDocumentImportResultDto> ImportAsync(bool ignoreEnabledFlag, CancellationToken cancellationToken);

    /// <summary>Arka plan servisinin bekleme süresi; ayar geçersizse varsayılan döner.</summary>
    Task<TimeSpan> GetIntervalAsync(CancellationToken cancellationToken);
}
