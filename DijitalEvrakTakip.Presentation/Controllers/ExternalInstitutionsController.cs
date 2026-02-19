using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetAllExternalInstitution;
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
}
