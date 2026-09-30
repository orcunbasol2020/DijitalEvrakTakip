using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.CreateDocumentAllocation;
using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.ReceiveDocumentAllocation;
using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveAllocationsByUserId;
using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveByDocumentId;
using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveDocumentsByUserId;
using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetAllocationTransferCountByUserId;
using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetByDocumentId;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class DocumentAllocationsController : ApiController
{
    public DocumentAllocationsController(IMediator mediator)
        : base(mediator) { }

    /// <summary>
    /// Kurum içi başka kullanıcıya Devir / Teslim onay talebi açar; diğer durumlarda zimmeti doğrudan oluşturur.
    /// Token gönderilirse createdUserId token'dan alınır.
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response =
            await _mediator.Send(
                request with { CreatedUserId = ResolveUserId(request.CreatedUserId) },
                cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Aktif zimmeti kendisinde olan kullanıcı evrağı teslim alır; evrak durumu Teslim Edildi olur.
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> Receive(
        ReceiveDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response =
            await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByDocumentId(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetDocumentAllocationByDocumentIdQuery(incomingDocumentId),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetActiveByDocumentId(
    Guid incomingDocumentId,
    CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetActiveDocumentAllocationByDocumentIdQuery(incomingDocumentId),
            cancellationToken);

        if (response == null)
        {
            return NotFound(new { Message = "Active document allocation not found." });
        }

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetActiveByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetActiveAllocationsByUserIdQuery(userId),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Kullanıcı üzerindeki aktif zimmetli gelen ve giden evrakları tek listede döner.
    /// documentDirection: 1 = Gelen, 2 = Giden, boş = ikisi birlikte. pageSize boşsa tüm kayıtlar döner.
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetActiveDocumentsByUserId(
        Guid userId,
        int? documentDirection,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetActiveDocumentsByUserIdQuery(userId, documentDirection, page, pageSize),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTransferCountByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllocationTransferCountByUserIdQuery(userId),
            cancellationToken);

        return Ok(response);
    }
}