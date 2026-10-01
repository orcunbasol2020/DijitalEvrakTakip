using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Commands.UpdateOutgoingDocumentShipment;

/// <summary>
/// Kargo kaydını günceller. Status "Teslim Edildi" olduğunda paketteki dağıtım
/// satırlarının teslim tarihi de doldurulur ve evraklara "Teslim Edildi" işlemi yazılır.
/// </summary>
public sealed record UpdateOutgoingDocumentShipmentCommand(
    Guid Id,
    int? CargoCompany,
    string? TrackingNumber,
    int? Status,
    DateTime? DeliveredDate,
    string? RecipientName,
    decimal? Cost,
    string? Notes,
    Guid? UpdatedUserId
) : IRequest<OutgoingDocumentShipmentDto>;
