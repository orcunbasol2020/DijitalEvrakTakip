using DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Commands.CreateOutgoingDocumentShipment;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Commands.UpdateOutgoingDocumentShipment;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentById;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentByTrackingNumber;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentsByDocumentId;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class OutgoingDocumentShipmentsController : ApiController
{
    public OutgoingDocumentShipmentsController(IMediator mediator) : base(mediator) { }

    // Seçilen dağıtım satırlarını kargoya verir; ilgili evrakların aktif zimmetini kapatır
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        [FromBody] CreateOutgoingDocumentShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateOutgoingDocumentShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById(
        [FromQuery] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOutgoingDocumentShipmentByIdQuery(id), cancellationToken);

        if (response is null)
            return NotFound(new { Message = "Kargo kaydı bulunamadı." });

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByOutgoingDocumentId(
        [FromQuery] Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetOutgoingDocumentShipmentsByDocumentIdQuery(outgoingDocumentId),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByTrackingNumber(
        [FromQuery] string trackingNumber,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetOutgoingDocumentShipmentByTrackingNumberQuery(trackingNumber),
            cancellationToken);

        if (response is null)
            return NotFound(new { Message = "Bu takip numarasına ait kargo kaydı bulunamadı." });

        return Ok(response);
    }
}
