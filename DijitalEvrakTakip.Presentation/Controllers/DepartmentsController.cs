using DijitalEvrakTakip.Application.Features.DeparmentFeatures.Commands.CreateDepartment;
using DijitalEvrakTakip.Application.Features.DepartmentFeatures.Queries.GetAllDepartment;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class DepartmentsController : ApiController
{
    public DepartmentsController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateDepartmentCommand request,
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
            new GetAllDepartmentQuery(),
            cancellationToken);

        return Ok(response);
    }
}
