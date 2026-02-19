using DijitalEvrakTakip.Application.Features.UserFeatures.Commands.CreateUser;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class UsersController : ApiController
{
    public UsersController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateUserCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllUserQuery request, CancellationToken cancellationToken)
    {
        IList<UserDto> response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    //[HttpPost("Login")]
    //public async Task<IActionResult> Login([FromBody] GetUserByUsernameQuery request, CancellationToken cancellationToken)
    //{
    //    var response = await _mediator.Send(request, cancellationToken);

    //    if (response == null)
    //        return Unauthorized();

    //    return Ok(response);
    //}

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (response == null)
            return Unauthorized();

        return Ok(response);
    }


}
