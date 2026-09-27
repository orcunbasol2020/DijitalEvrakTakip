namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record OutgoingDocumentDistributionDto(
    Guid Id,
    Guid OutgoingDocumentId,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? ExternalInstitutionId,
    string? ExternalInstitutionName,
    bool? ActionRequired,
    int? DeliveryMethod,
    DateTime? SentDate,
    DateTime? DeliveryDate,
    Guid? ShipmentId,
    int? CargoCompany,
    string? TrackingNumber,
    int? ShipmentStatus,
    string? Notes,
    DateTime CreatedDate,
    DateTime? UpdateDate
);
