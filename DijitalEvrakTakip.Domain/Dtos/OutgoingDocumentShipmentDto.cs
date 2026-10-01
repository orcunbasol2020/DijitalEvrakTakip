namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record OutgoingDocumentShipmentDto(
    Guid Id,
    int CargoCompany,
    string CargoCompanyName,
    string TrackingNumber,
    DateTime SentDate,
    Guid SentUserId,
    string? SentUserFullName,
    Guid? ExternalInstitutionId,
    string? ExternalInstitutionName,
    string? RecipientName,
    int Status,
    string StatusName,
    DateTime? DeliveredDate,
    decimal? Cost,
    string? Notes,
    IList<OutgoingDocumentShipmentItemDto> Items,
    DateTime CreatedDate,
    DateTime? UpdateDate
);

// Paketin içindeki dağıtım satırları (hangi evrak, hangi alıcı)
public sealed record OutgoingDocumentShipmentItemDto(
    Guid DistributionId,
    Guid OutgoingDocumentId,
    string? DocumentNumber,
    string? Subject,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? ExternalInstitutionId,
    string? ExternalInstitutionName
);
