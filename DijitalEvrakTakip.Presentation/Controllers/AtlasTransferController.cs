using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RetryAtlasTransfer;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RunAtlasTransfer;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferStats;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetFailedAtlasTransfers;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

/// <summary>
/// Yayınlanan gelen evrakların EYP olarak Atlas'a aktarım kuyruğu.
/// </summary>
public sealed class AtlasTransferController : ApiController
{
    public AtlasTransferController(IMediator mediator)
        : base(mediator) { }

    // Yayın durumlarına göre evrak sayıları
    [HttpGet("[action]")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAtlasTransferStatsQuery(), cancellationToken);

        return Ok(response);
    }

    // Aktarımı hatalı olan evraklar ve son hata mesajları
    [HttpGet("[action]")]
    public async Task<IActionResult> GetFailed(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetFailedAtlasTransfersQuery(), cancellationToken);

        return Ok(response);
    }

    // Hatalı aktarımı evrakın güncel bilgileriyle yeniden kuyruğa alır
    [HttpPost("[action]")]
    public async Task<IActionResult> Retry(
        RetryAtlasTransferCommand request,
        CancellationToken cancellationToken)
    {
        var command = request with { UserId = ResolveUserId(request.UserId) };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    // Arka plan servisini beklemeden bir tur çalıştırır (otomatik aktarım kapalı olsa da)
    [HttpPost("[action]")]
    public async Task<IActionResult> Run(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RunAtlasTransferCommand(), cancellationToken);

        return Ok(response);
    }
}
