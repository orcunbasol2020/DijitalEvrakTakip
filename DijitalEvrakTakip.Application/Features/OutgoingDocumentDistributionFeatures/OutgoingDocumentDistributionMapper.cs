using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures;

internal static class OutgoingDocumentDistributionMapper
{
    public static OutgoingDocumentDistributionDto ToDto(OutgoingDocumentDistribution x) => new(
        x.Id,
        x.OutgoingDocumentId,
        x.DepartmentId,
        x.Department?.Name,
        x.ExternalInstitutionId,
        x.ExternalInstitution?.Name,
        x.ActionRequired,
        x.DeliveryMethod,
        x.SentDate,
        x.DeliveryDate,
        x.ShipmentId,
        x.Shipment?.CargoCompany,
        x.Shipment?.TrackingNumber,
        x.Shipment?.Status,
        x.Notes,
        x.CreatedDate,
        x.UpdateDate
    );
}
