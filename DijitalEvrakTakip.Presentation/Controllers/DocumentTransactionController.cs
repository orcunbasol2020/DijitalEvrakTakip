
using DijitalEvrakTakip.Application.Features.DocumentTransactionFeatures.Commands.CreateDocumentTransaction;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class DocumentTransactionsController : ApiController
{
    public DocumentTransactionsController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateDocumentTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{documentId}")]
    public async Task<IActionResult> GetByDocumentId(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetDocumentTransactionsByDocumentIdQuery(documentId),
            cancellationToken);

        return Ok(response);
    }
}
