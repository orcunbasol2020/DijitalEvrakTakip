using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IOutgoingDocumentShipmentService
{
    Task CreateAsync(OutgoingDocumentShipment shipment, CancellationToken cancellationToken);

    Task UpdateAsync(OutgoingDocumentShipment shipment, CancellationToken cancellationToken);

    Task<OutgoingDocumentShipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    // Güncelleme için: navigation yüklenmez
    Task<OutgoingDocumentShipment?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken);

    Task<OutgoingDocumentShipmentDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<OutgoingDocumentShipmentDto?> GetDtoByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken);

    Task<IList<OutgoingDocumentShipmentDto>> GetDtosByOutgoingDocumentIdAsync(Guid outgoingDocumentId, CancellationToken cancellationToken);
}
