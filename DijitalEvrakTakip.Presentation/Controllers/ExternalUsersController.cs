using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.CreateExternalUser;
using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetAllExternalUser;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class ExternalUsersController : ApiController
{
    public ExternalUsersController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateExternalUserCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllExternalUserQuery request, CancellationToken cancellationToken)
    {
        IList<ExternalUserDto> response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}