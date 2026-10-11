namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record EnvelopeDocumentCountDto(
    Guid Id,
    string EnvelopeNo,
    bool IsClosed,
    int Status,
    Guid CreatedByUserId,
    Guid? ExternalInstitutionId,
    string? ExternalInstitutionName,
    int? DepartmentId,
    int? TargetDepartmentId,
    string? UnitName,
    string? Address,
    DateTime CreatedDate,
    int DocumentCount
);