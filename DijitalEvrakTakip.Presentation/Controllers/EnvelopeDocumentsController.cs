using DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Commands.CreateEnvelopeDocument;
using DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Commands.RemoveEnvelopeDocument;
using DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Queries.GetAllEnvelopeDocument;
using DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Queries.GetEnvelopeDocumentsByEnvelopeId;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class EnvelopeDocumentsController : ApiController
{
    public EnvelopeDocumentsController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateEnvelopeDocumentCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllEnvelopeDocumentQuery(),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByEnvelopeId(
    Guid envelopeId,
    CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetEnvelopeDocumentsByEnvelopeIdQuery(envelopeId),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Remove(
    RemoveEnvelopeDocumentCommand request,
    CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}