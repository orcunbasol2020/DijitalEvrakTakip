using DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.CreateOutgoingDocumentDistribution;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.RemoveOutgoingDocumentDistribution;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.UpdateOutgoingDocumentDistribution;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Queries.GetOutgoingDocumentDistributionsByDocumentId;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class OutgoingDocumentDistributionsController : ApiController
{
    public OutgoingDocumentDistributionsController(IMediator mediator) : base(mediator) { }

    // Bir evrak için bir veya birden fazla alıcı ekler
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        [FromBody] CreateOutgoingDocumentDistributionCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateOutgoingDocumentDistributionCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByOutgoingDocumentId(
        [FromQuery] Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetOutgoingDocumentDistributionsByDocumentIdQuery(outgoingDocumentId),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Remove(
        [FromBody] RemoveOutgoingDocumentDistributionCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}
