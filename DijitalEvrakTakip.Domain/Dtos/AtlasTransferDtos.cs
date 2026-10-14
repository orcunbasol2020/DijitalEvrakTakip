namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record AtlasTransferRunResultDto(
    // Tur çalıştı mı (kapalıysa, ayar eksikse veya başka tur sürüyorsa false)
    bool Ran,
    // Kuyruktan alınan evrak sayısı
    int Picked,
    int Succeeded,
    // Geçici hata nedeniyle tekrar kuyruğa dönenler
    int Requeued,
    // Hatalı durumuna düşenler
    int Failed,
    // Uzun süre Aktarılıyor'da kalıp kuyruğa geri alınanlar
    int RecoveredStuck,
    string Message
);

public sealed record AtlasTransferStatsDto(
    int NotPublished,
    int Queued,
    int Transferring,
    int Succeeded,
    int Failed
);

public sealed record AtlasTransferFailedDocumentDto(
    Guid DocumentId,
    string QrCode,
    string? Subject,
    int TryCount,
    string? LastError,
    DateTime? LastAttemptAt
);

/// <summary>Atlas'a gönderilecek tamamlanmış EYP paketi.</summary>
public sealed record AtlasEypSubmissionDto(
    Guid DocumentId,
    string QrCode,
    string FileName,
    byte[] Content
);

/// <summary>Atlas'a gönderimin sonucu.</summary>
public sealed record AtlasEypTransferResultDto(
    bool IsSuccess,
    // true ise tekrar denenmez (ör. Atlas paketi geçersiz bularak reddetti)
    bool IsPermanentFailure,
    string? AtlasReferenceId,
    string? Message)
{
    public static AtlasEypTransferResultDto Success(string? atlasReferenceId) =>
        new(true, false, atlasReferenceId, null);

    public static AtlasEypTransferResultDto TransientFailure(string message) =>
        new(false, false, null, message);

    public static AtlasEypTransferResultDto Rejected(string message) =>
        new(false, true, null, message);
}

/// <summary>
/// Aktarım servisinin anlık durumu: ayarlar, son ve sonraki tur, ön koşul kontrolleri ve sayılar.
/// Son/sonraki tur bilgisi uygulama belleğindedir; uygulama yeniden başlayınca ilk tura kadar boştur.
/// Tüm zamanlar UTC.
/// </summary>
public sealed record AtlasTransferStatusDto(
    bool AutoTransferEnabled,
    int IntervalSeconds,
    int BatchSize,
    int MaxTryCount,
    // Bir tur şu anda çalışıyor mu ve ne zamandan beri
    bool IsRunning,
    DateTime? RunningSince,
    // Tamamlanan son tur (arka plan servisi veya elle tetikleme)
    AtlasTransferLastRunDto? LastRun,
    // Arka plan servisinin bir sonraki turu
    DateTime? NextRunAt,
    // Veritabanından: son paket gönderim denemesi ve son başarılı aktarım
    DateTime? LastAttemptAt,
    DateTime? LastSuccessAt,
    AtlasTransferStatsDto Counts,
    // Aktarımın çalışması için gereken koşullar; Ok=false olanlar sorun gösterir
    IReadOnlyList<AtlasTransferCheckDto> Checks
);

public sealed record AtlasTransferLastRunDto(
    DateTime StartedAt,
    DateTime FinishedAt,
    // true: Run uç noktasıyla elle tetiklendi; false: arka plan servisi
    bool Manual,
    // Tur tamamlandıysa sonucu; beklenmeyen bir hatayla kesildiyse null ve Error dolu
    AtlasTransferRunResultDto? Result,
    string? Error
);

/// <summary>Aktarımın ön koşullarından biri. Code sabittir, frontend eşleştirmek için kullanabilir.</summary>
public sealed record AtlasTransferCheckDto(
    string Code,
    string Name,
    bool Ok,
    string Message
);

/// <summary>Kuyrukta bekleyen veya o anda aktarılan evrak.</summary>
public sealed record AtlasTransferQueueItemDto(
    Guid DocumentId,
    string QrCode,
    string? Subject,
    // PublishStatusEnum: 2 Kuyrukta, 3 Aktarılıyor
    int Status,
    DateTime? StatusUpdatedAt,
    int TryCount,
    // Geçici hatadan sonra bir sonraki denemenin en erken zamanı; null ise ilk turda alınır
    DateTime? NextAttemptAt,
    string? LastError
);

/// <summary>Bir paket gönderim denemesi (oluşturma + gönderim).</summary>
public sealed record AtlasTransferAttemptDto(
    Guid DocumentId,
    string QrCode,
    string? Subject,
    Guid PackageId,
    int AttemptNo,
    DateTime AttemptedAt,
    bool IsSuccess,
    // Tekrar denenmeyecek hata (EYP üretilemedi, Atlas reddetti, deneme sınırı doldu)
    bool IsPermanentFailure,
    string? Error,
    string? AtlasReferenceId
);

/// <summary>Evrakın EYP paketi.</summary>
public sealed record AtlasTransferPackageDto(
    Guid Id,
    // false: evrak yeniden kuyruğa alındığı için yerine yeni paket üretilen eski paket
    bool IsActive,
    DateTime CreatedDate,
    string? FileName,
    long? FileSize,
    string? Sha256,
    // Paket dosyası diskte duruyor mu (indirilebilir mi)
    bool FileExists,
    // Evrakın dosyası olmadığı için üst yazı olarak fiziki gönderim PDF'i kullanıldı
    bool UsedPlaceholderContent,
    int TryCount,
    DateTime? LastAttemptAt,
    DateTime? NextAttemptAt,
    string? LastError,
    string? AtlasReferenceId,
    DateTime? TransferredAt
);

/// <summary>Evrakın Atlas aktarım durumu, paketleri (en yenisi önce) ve tüm denemeleri (en yenisi önce).</summary>
public sealed record AtlasTransferDocumentDto(
    Guid DocumentId,
    string QrCode,
    string? Subject,
    // PublishStatusEnum; eski kayıtlarda null (yayınlanmamış sayılır)
    int? Status,
    DateTime? StatusUpdatedAt,
    IReadOnlyList<AtlasTransferPackageDto> Packages,
    IReadOnlyList<AtlasTransferAttemptDto> Attempts
);

/// <summary>İndirilecek EYP paket dosyası.</summary>
public sealed record AtlasTransferPackageFileDto(
    string FileName,
    byte[] Content
);
