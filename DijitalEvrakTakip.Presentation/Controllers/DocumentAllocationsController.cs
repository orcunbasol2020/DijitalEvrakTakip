using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.CreateDocumentAllocation;
using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveAllocationsByUserId;
using DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveByDocumentId;
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

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateDocumentAllocationCommand request,
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