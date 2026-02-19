using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.PreRegisterIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentByQrCode;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;


namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class IncomingDocumentsController : ApiController
{
    public IncomingDocumentsController(IMediator mediator) : base(mediator)
    {
    }

    // CREATE
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        [FromBody] CreateIncomingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    // PRE REGISTER
    [HttpPost("[action]")]
    public async Task<IActionResult> PreRegister(
        [FromBody] PreRegisterIncomingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(request, cancellationToken);

        return Ok(new { created });
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByQrCode([FromQuery] string qrCode, CancellationToken cancellationToken)
    {
        var query = new GetIncomingDocumentByqrCodeQuery(qrCode);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);

    }

    // UPDATE
    [HttpPut("[action]")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateIncomingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }


    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll([FromQuery] string? Status, CancellationToken cancellationToken)
    {
        var query = new GetAllIncomingDocumentQuery
        {
            Status = Status
        };

        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }


}
