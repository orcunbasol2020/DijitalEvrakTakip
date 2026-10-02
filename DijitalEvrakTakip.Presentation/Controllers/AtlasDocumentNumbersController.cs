using DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.CancelAtlasDocumentNumber;
using DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.RefillAtlasDocumentNumberPool;
using DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.ReserveAtlasDocumentNumbers;
using DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Queries.GetAtlasDocumentNumberPoolStock;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

/// <summary>
/// Atlas EBYS'den önceden alınan evrak numarası (QR kod) havuzu.
/// </summary>
public sealed class AtlasDocumentNumbersController : ApiController
{
    public AtlasDocumentNumbersController(IMediator mediator)
        : base(mediator) { }

    // Etiket basmak için havuzdan numara ayırır
    [HttpPost("[action]")]
    public async Task<IActionResult> Reserve(
        ReserveAtlasDocumentNumbersCommand request,
        CancellationToken cancellationToken)
    {
        var command = request with { UserId = ResolveUserId(request.UserId) };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetStock(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAtlasDocumentNumberPoolStockQuery(), cancellationToken);

        return Ok(response);
    }

    // Havuzu hemen kontrol eder (otomatik doldurma kapalı olsa da çalışır)
    [HttpPost("[action]")]
    public async Task<IActionResult> Refill(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RefillAtlasDocumentNumberPoolCommand(), cancellationToken);

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Cancel(
        CancelAtlasDocumentNumberCommand request,
        CancellationToken cancellationToken)
    {
        var command = request with { UserId = ResolveUserId(request.UserId ?? "") };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }
}
