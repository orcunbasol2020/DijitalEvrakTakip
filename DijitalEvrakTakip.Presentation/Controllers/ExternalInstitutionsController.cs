using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetAllExternalInstitution;
using DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetExternalInstitutionById;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

/// <summary>
/// Dış kurumlar yalnızca okunur (ComAgency); ekleme / güncelleme / silme kaynak sistemde yapılır.
/// </summary>
public sealed class ExternalInstitutionsController : ApiController
{
    public ExternalInstitutionsController(IMediator mediator) : base(mediator) { }

    // GET: api/ExternalInstitutions/GetAll?type=1&search=...&take=50
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllExternalInstitutionQuery request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById(
        int id,
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
