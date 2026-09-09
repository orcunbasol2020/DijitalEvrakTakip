using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetAllExternalInstitution;
using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetExternalInstitutionById;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class ExternalInstitutionsController : ApiController
{
    public ExternalInstitutionsController(IMediator mediator) : base(mediator) { }

    // GET: api/ExternalInstitutions/GetAll
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllExternalInstitutionQuery(),
            cancellationToken
        );

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetExternalInstitutionByIdQuery(id),
            cancellationToken
        );

        if (response == null)
            return NotFound($"External institution with Id '{id}' not found.");

        return Ok(response);
    }
}
