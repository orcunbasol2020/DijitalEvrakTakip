using DijitalEvrakTakip.Application.Features.DocumentAssignmentFeatures.Commands.CreateDocumentAssignment;
using DijitalEvrakTakip.Application.Features.DocumentAssignmentFeatures.Queries.GetByDocumentId;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class DocumentAssignmentsController : ApiController
{
    public DocumentAssignmentsController(IMediator mediator)
        : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateDocumentAssignmentCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response =
            await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByDocumentId(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetDocumentAssignmentByDocumentIdQuery(documentId),
            cancellationToken);

        return Ok(response);
    }
}