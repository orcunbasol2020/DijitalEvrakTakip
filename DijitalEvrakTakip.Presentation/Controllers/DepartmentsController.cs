using DijitalEvrakTakip.Application.Features.DepartmentFeatures.Queries.GetAllDepartment;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class DepartmentsController : ApiController
{
    public DepartmentsController(IMediator mediator) : base(mediator) { }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeMissions,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllDepartmentQuery(includeMissions),
            cancellationToken);

        return Ok(response);
    }
}
