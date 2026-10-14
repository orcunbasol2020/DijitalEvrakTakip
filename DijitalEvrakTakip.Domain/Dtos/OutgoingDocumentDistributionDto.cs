namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record OutgoingDocumentDistributionDto(
    Guid Id,
    Guid OutgoingDocumentId,
    int? DepartmentId,
    string? DepartmentName,
    int? ExternalInstitutionId,
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
