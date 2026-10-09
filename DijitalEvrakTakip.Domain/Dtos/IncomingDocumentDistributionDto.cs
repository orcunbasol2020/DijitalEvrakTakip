namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record IncomingDocumentDistributionDto(
    Guid Id,
    Guid IncomingDocumentId,
    Guid DepartmentId,
    string? DepartmentName,
    bool? ActionRequired,
    string? Notes,
    DateTime CreatedDate,
    DateTime? UpdateDate
);
