using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentByTrackingNumber;

public sealed class GetOutgoingDocumentShipmentByTrackingNumberHandler
    : IRequestHandler<GetOutgoingDocumentShipmentByTrackingNumberQuery, OutgoingDocumentShipmentDto?>
{
    private readonly IOutgoingDocumentShipmentService _shipmentService;

    public GetOutgoingDocumentShipmentByTrackingNumberHandler(IOutgoingDocumentShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    public Task<OutgoingDocumentShipmentDto?> Handle(
        GetOutgoingDocumentShipmentByTrackingNumberQuery request,
        CancellationToken cancellationToken)
    {
        return _shipmentService.GetDtoByTrackingNumberAsync(request.TrackingNumber.Trim(), cancellationToken);
    }
}
