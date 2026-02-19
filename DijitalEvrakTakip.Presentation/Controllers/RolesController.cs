using DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.CreateRole;
using DijitalEvrakTakip.Application.Features.RoleFeatures.Queries.GetAllRole;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class RolesController : ApiController
{
    public RolesController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRoleQuery request, CancellationToken cancellationToken)
    {
        IList<Role> response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}
