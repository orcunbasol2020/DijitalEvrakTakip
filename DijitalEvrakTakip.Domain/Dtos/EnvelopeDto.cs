namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record EnvelopeDto(
    Guid Id,
    string EnvelopeNo,
    bool IsClosed,
    int Status,
    Guid CreatedByUserId,
    Guid? ExternalInstitutionId,
    Guid? DepartmentId,
    Guid? TargetDepartmentId,
    string? UnitName,
    string? Address,
    DateTime CreatedDate
);