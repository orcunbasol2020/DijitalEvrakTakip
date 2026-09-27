namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record ScannedDocumentImportResultDto(
    // İçe aktarma çalıştı mı (kapalıysa veya klasör yoksa false)
    bool Ran,
    int Imported,
    int Skipped,
    int Failed,
    string Message
);
