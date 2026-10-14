namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record IncomingDocumentAttachmentDto(
    Guid Id,
    Guid IncomingDocumentId,
    string Name,
    string? Description,
    int OrderNo,
    string OriginalFileName,
    long FileSize,
    string? UploadedUserId,
    DateTime CreatedDate,
    DateTime? UpdateDate
);
