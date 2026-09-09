using DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.CreateEnvelope;
using DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetAllEnvelope;
using DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetEnvelopeById;
using DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetEnvelopeByNo;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class EnvelopesController : ApiController
{
    public EnvelopesController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateEnvelopeCommand request,
        CancellationToken cancellationToken)
    {
        EnvelopeReturnDto response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllEnvelopeQuery(),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByNo(
    string envelopeNo,
    CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetEnvelopeByNoQuery(envelopeNo),
            cancellationToken);

        if (response == null)
            return NotFound($"Envelope with No '{envelopeNo}' not found.");

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById(
     Guid id,
     CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetEnvelopeByIdQuery(id),
            cancellationToken);

        if (response == null)
            return NotFound($"Envelope with Id '{id}' not found.");

        return Ok(response);
    }
}