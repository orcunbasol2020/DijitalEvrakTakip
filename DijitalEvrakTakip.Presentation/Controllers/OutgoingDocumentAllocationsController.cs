using DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.CreateOutgoingDocumentAllocation;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.UpdateOutgoingDocumentAllocation;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetActiveAllocationsByUserId;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetActiveByDocumentId;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetAllocationTransferCountByUserId;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetByDocumentId;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class OutgoingDocumentAllocationsController : ApiController
{
    public OutgoingDocumentAllocationsController(IMediator mediator)
        : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateOutgoingDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response =
            await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Update(
        UpdateOutgoingDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response =
            await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByDocumentId(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetOutgoingDocumentAllocationByDocumentIdQuery(outgoingDocumentId),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetActiveByDocumentId(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetActiveOutgoingDocumentAllocationByDocumentIdQuery(outgoingDocumentId),
            cancellationToken);

        if (response == null)
        {
            return NotFound(new { Message = "Active outgoing document allocation not found." });
        }

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetActiveByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetActiveOutgoingAllocationsByUserIdQuery(userId),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTransferCountByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetOutgoingAllocationTransferCountByUserIdQuery(userId),
            cancellationToken);

        return Ok(response);
    }
}
