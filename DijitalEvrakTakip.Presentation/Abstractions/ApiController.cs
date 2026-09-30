using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DijitalEvrakTakip.Presentation.Abstractions;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    public readonly IMediator _mediator;

    protected ApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Geçerli token gönderildiyse işlemi yapan kullanıcı token'dan alınır, gövdedeki değer yok sayılır.
    // Token yoksa (endpoint'ler henüz [Authorize] değil) gövdedeki değer kullanılır.
    protected string ResolveUserId(string bodyUserId)
    {
        if (User.Identity?.IsAuthenticated != true)
            return bodyUserId;

        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? bodyUserId;
    }
}
