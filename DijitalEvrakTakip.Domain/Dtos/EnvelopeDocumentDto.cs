namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record EnvelopeDocumentDto(
    Guid Id,
    Guid EnvelopeId,
    string QrCode,
    Guid? DocumentId,
    bool IsDeleted,
    DateTime CreatedDate,
    DateTime? UpdateDate
);