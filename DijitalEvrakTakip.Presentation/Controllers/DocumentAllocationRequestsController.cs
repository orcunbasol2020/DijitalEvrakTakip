using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequest;
using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequests;
using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.CancelAllocationRequest;
using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.CancelAllocationRequests;
using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.RejectAllocationRequest;
using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.RejectAllocationRequests;
using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetAllocationRequestsByDocumentId;
using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetPendingAllocationRequestsByUserId;
using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetSentAllocationRequestsByUserId;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

/// <summary>
/// Gelen evrakta kurum içi Devir / Teslim onay talepleri. Talepler DocumentAllocations/Create ile oluşur.
/// Tekli işlemler data içinde { requestId, result, message }, toplu işlemler bunların listesini döner.
/// result: 1 Başarılı, 2 Geçersiz, 3 Çakışma, 4 Bulunamadı, 5 Yetkisiz, 6 Onay Beklemiyor, 7 Geçersiz UserId.
/// Token gönderilirse userId token'dan alınır.
/// </summary>
public sealed class DocumentAllocationRequestsController : ApiController
{
    public DocumentAllocationRequestsController(IMediator mediator)
        : base(mediator) { }

    /// <summary>
    /// Alıcı zimmeti kabul eder; Teslim talebi Teslim Alındı, Devir talebi Devir Alındı olarak üzerine geçer.
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> Approve(
        ApproveAllocationRequestCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(
            request with { UserId = ResolveUserId(request.UserId) },
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Birden fazla talebi onaylar (en fazla 500). Devredene kişi başına tek özet bildirim gider.
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> ApproveBulk(
        ApproveAllocationRequestsCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(
            request with { UserId = ResolveUserId(request.UserId) },
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Alıcı zimmeti reddeder; evrak devredende kalır.
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> Reject(
        RejectAllocationRequestCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(
            request with { UserId = ResolveUserId(request.UserId) },
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Birden fazla talebi ortak gerekçeyle reddeder (en fazla 500). Devredene kişi başına tek özet bildirim gider.
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> RejectBulk(
        RejectAllocationRequestsCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(
            request with { UserId = ResolveUserId(request.UserId) },
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Devreden veya işlemi yapan kullanıcı bekleyen talebi geri çeker.
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> Cancel(
        CancelAllocationRequestCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(
            request with { UserId = ResolveUserId(request.UserId) },
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Birden fazla talebi ortak gerekçeyle geri çeker (en fazla 500). Alıcıya kişi başına tek özet bildirim gider.
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> CancelBulk(
        CancelAllocationRequestsCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(
            request with { UserId = ResolveUserId(request.UserId) },
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Kullanıcının onayını bekleyen talepler (eskiden yeniye).
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPendingByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetPendingAllocationRequestsByUserIdQuery(userId),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Kullanıcının devrettiği veya işlemini yaptığı talepler (yeniden eskiye).
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetSentByUserId(
        Guid userId,
        bool onlyPending,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetSentAllocationRequestsByUserIdQuery(userId, onlyPending),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByDocumentId(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllocationRequestsByDocumentIdQuery(incomingDocumentId),
            cancellationToken);

        return Ok(response);
    }
}
