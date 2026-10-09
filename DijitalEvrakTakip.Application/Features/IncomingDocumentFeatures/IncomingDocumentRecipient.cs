namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures;

/// <summary>
/// Gelen evrağın "Nereye" listesindeki tek bir iç birim. Bilgi/Gereği alıcı bazındadır:
/// true = Gereği, false = Bilgi, null = belirtilmemiş (EYP'de Gereği sayılır).
/// </summary>
public sealed record IncomingDocumentRecipient(
    Guid DepartmentId,
    bool? ActionRequired,
    string? Notes = null
);
