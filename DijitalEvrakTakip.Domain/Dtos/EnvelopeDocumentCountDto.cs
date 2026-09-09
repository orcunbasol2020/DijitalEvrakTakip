namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record EnvelopeDocumentCountDto(
    Guid Id,
    string EnvelopeNo,
    bool IsClosed,
    Guid CreatedByUserId,
    Guid? ExternalInstitutionId,
    string? ExternalInstitutionName,
    Guid? DepartmentId,
    string? UnitName,
    string? Address,
    DateTime CreatedDate,
    int DocumentCount
);