using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentsByDocumentId;

public sealed class GetOutgoingDocumentShipmentsByDocumentIdHandler
    : IRequestHandler<GetOutgoingDocumentShipmentsByDocumentIdQuery, IList<OutgoingDocumentShipmentDto>>
{
    private readonly IOutgoingDocumentShipmentService _shipmentService;

    public GetOutgoingDocumentShipmentsByDocumentIdHandler(IOutgoingDocumentShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    public Task<IList<OutgoingDocumentShipmentDto>> Handle(
        GetOutgoingDocumentShipmentsByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        return _shipmentService.GetDtosByOutgoingDocumentIdAsync(request.OutgoingDocumentId, cancellationToken);
    }
}
