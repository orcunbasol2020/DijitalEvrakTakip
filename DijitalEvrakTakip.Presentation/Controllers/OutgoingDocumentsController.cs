using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.CreateOutgoingDocument;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.UpdateOutgoingDocument;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetAllOutgoingDocument;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentById;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentByQrCode;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentTransactionsByDocumentId;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class OutgoingDocumentsController : ApiController
{
    public OutgoingDocumentsController(IMediator mediator) : base(mediator)
    {
    }

    // CREATE
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        [FromBody] CreateOutgoingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    // UPDATE
    [HttpPut("[action]")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateOutgoingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOutgoingDocumentByIdQuery(id);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByQrCode([FromQuery] string qrCode, CancellationToken cancellationToken)
    {
        var query = new GetOutgoingDocumentByQrCodeQuery(qrCode);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll([FromQuery] int? status, CancellationToken cancellationToken)
    {
        var query = new GetAllOutgoingDocumentQuery
        {
            Status = status
        };

        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTransactionHistory(
        [FromQuery] Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var query = new GetOutgoingDocumentTransactionsByDocumentIdQuery(outgoingDocumentId);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
