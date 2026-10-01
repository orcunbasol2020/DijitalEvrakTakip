using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.CreateExternalUser;
using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.DeleteExternalUser;
using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.UpdateExternalUser;
using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetAllExternalUser;
using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetExternalUserById;
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

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetExternalUserByIdQuery(id), cancellationToken);

        if (response == null)
            return NotFound($"External user with Id '{id}' not found.");

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Update(UpdateExternalUserCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Delete(DeleteExternalUserCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}