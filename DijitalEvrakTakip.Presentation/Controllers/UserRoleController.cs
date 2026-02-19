using DijitalEvrakTakip.Application.Features.UserRoleFeatures.Commands.CreateUserRole;
using DijitalEvrakTakip.Application.Features.UserRoleFeatures.Queries.GetUserRoleByUser;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class UserRoleController : ApiController
{
    public UserRoleController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByUser([FromQuery] GetUserRoleByUserQuery request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }
}
