using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentById;

public sealed class GetOutgoingDocumentShipmentByIdHandler
    : IRequestHandler<GetOutgoingDocumentShipmentByIdQuery, OutgoingDocumentShipmentDto?>
{
    private readonly IOutgoingDocumentShipmentService _shipmentService;

    public GetOutgoingDocumentShipmentByIdHandler(IOutgoingDocumentShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    public Task<OutgoingDocumentShipmentDto?> Handle(
        GetOutgoingDocumentShipmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        return _shipmentService.GetDtoByIdAsync(request.Id, cancellationToken);
    }
}
