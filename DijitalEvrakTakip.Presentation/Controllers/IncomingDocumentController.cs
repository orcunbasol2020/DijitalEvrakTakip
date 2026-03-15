using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.PreRegisterIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentById;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentByQrCode;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentLast30DaysStats;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentTodayStats;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetPendingIncomingDocumentCount;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;


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
        var id = await _mediator.Send(request, cancellationToken);

        return Ok(new { id });
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByQrCode([FromQuery] string qrCode, CancellationToken cancellationToken)
    {
        var query = new GetIncomingDocumentByqrCodeQuery(qrCode);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);

    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetIncomingDocumentByIdQuery(id);
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

    [HttpGet("[action]")]
    public async Task<IActionResult> GetPendingCount([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetPendingIncomingDocumentCountQuery(userId);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTodayStats(CancellationToken cancellationToken)
    {
        var query = new GetIncomingDocumentTodayStatsQuery();
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetLast30DaysStats(CancellationToken cancellationToken)
    {
        var query = new GetIncomingDocumentLast30DaysStatsQuery();
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
