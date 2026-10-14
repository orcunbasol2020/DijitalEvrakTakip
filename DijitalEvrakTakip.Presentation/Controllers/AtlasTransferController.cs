using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RetryAtlasTransfer;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RunAtlasTransfer;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferAttempts;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferDocument;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferPackageFile;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferQueue;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferStats;
using DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferStatus;
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

    // Servisin durumu: ayarlar, çalışan / son / sonraki tur, ön koşul kontrolleri ve sayılar
    [HttpGet("[action]")]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAtlasTransferStatusQuery(), cancellationToken);

        return Ok(response);
    }

    // Kuyrukta bekleyen ve o anda aktarılan evraklar, sıraya alınma sırasıyla
    [HttpGet("[action]")]
    public async Task<IActionResult> GetQueue(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAtlasTransferQueueQuery(), cancellationToken);

        return Ok(response);
    }

    // Tüm evraklardaki son gönderim denemeleri, en yenisi önce (limit 1–500, varsayılan 50)
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAttempts([FromQuery] int limit = 50, CancellationToken cancellationToken = default)
    {
        var response = await _mediator.Send(new GetAtlasTransferAttemptsQuery(limit), cancellationToken);

        return Ok(response);
    }

    // Evrakın aktarım durumu, EYP paketleri ve tüm denemeleri (evrak detay ekranı için)
    [HttpGet("[action]/{documentId:guid}")]
    public async Task<IActionResult> GetDocument(Guid documentId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAtlasTransferDocumentQuery(documentId), cancellationToken);

        return Ok(response);
    }

    // Evrakın EYP paket dosyasını indirir; packageId verilmezse güncel paket
    [HttpGet("[action]/{documentId:guid}")]
    public async Task<IActionResult> DownloadPackage(
        Guid documentId,
        [FromQuery] Guid? packageId,
        CancellationToken cancellationToken)
    {
        var file = await _mediator.Send(new GetAtlasTransferPackageFileQuery(documentId, packageId), cancellationToken);

        return File(file.Content, "application/octet-stream", file.FileName);
    }

    // Arka plan servisini beklemeden bir tur çalıştırır (otomatik aktarım kapalı olsa da)
    [HttpPost("[action]")]
    public async Task<IActionResult> Run(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RunAtlasTransferCommand(), cancellationToken);

        return Ok(response);
    }
}
