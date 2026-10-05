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
