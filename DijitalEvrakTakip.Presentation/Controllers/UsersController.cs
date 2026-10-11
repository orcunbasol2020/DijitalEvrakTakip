using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserById;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUsersByParentDepartment;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetDepartmentUsersLoginStatus;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.WindowsLogin;
using DijitalEvrakTakip.Application.Options;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class UsersController : ApiController
{
    public UsersController(IMediator mediator) : base(mediator) { }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllUserQuery request, CancellationToken cancellationToken)
    {
        IList<UserDto> response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Yöneticinin departmanı (departmentId) ve altındaki tüm alt departmanlardaki personelleri getirir.
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetByParentDepartment(
        [FromQuery] GetUsersByParentDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        IList<UserDto> response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Birimdeki personelleri oturum (login) durumlarıyla birlikte getirir.
    /// Son başarılı giriş JWT geçerlilik süresi içindeyse IsLoggedIn=true döner.
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetDepartmentUsersLoginStatus(
        [FromQuery] GetDepartmentUsersLoginStatusQuery request,
        CancellationToken cancellationToken)
    {
        IList<DepartmentUserLoginStatusDto> response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        if (response == null)
            return NotFound($"User with Id '{id}' not found.");

        return Ok(response);
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] GetUserByUsernameQuery request,
        [FromServices] IOptions<LoginSettings> loginSettings,
        CancellationToken cancellationToken)
    {
        if (!loginSettings.Value.FormLoginEnabled)
            return NotFound();

        // IP ve UserAgent istemciden alınmaz, sunucu tarafında HttpContext'ten doldurulur (login logu için).
        request = request with
        {
            IpAddress = GetClientIpAddress(),
            UserAgent = Request.Headers["User-Agent"].ToString()
        };

        var response = await _mediator.Send(request, cancellationToken);

        if (response == null)
            return Unauthorized();

        return Ok(response);
    }

    /// <summary>
    /// Windows oturumundaki kullanıcıyla (Kerberos) şifresiz giriş; cevap Login ile aynıdır (token dahil).
    /// İstemci withCredentials ile çağırmalı. Kimlik doğrulanamazsa 401, kullanıcı sistemde yoksa,
    /// silinmiş ya da pasifse 403 döner.
    /// </summary>
    [HttpPost("[action]")]
    [Authorize(AuthenticationSchemes = LoginSettings.WindowsScheme)]
    public async Task<IActionResult> WindowsLogin(
        [FromServices] IOptions<LoginSettings> loginSettings,
        CancellationToken cancellationToken)
    {
        if (!loginSettings.Value.WindowsLoginEnabled)
            return NotFound();

        var query = new WindowsLoginQuery(
            User.Identity?.Name,
            GetClientIpAddress(),
            Request.Headers["User-Agent"].ToString());

        var response = await _mediator.Send(query, cancellationToken);

        if (response == null)
            return StatusCode(StatusCodes.Status403Forbidden);

        return Ok(response);
    }

    /// <summary>
    /// Ön yüzün hangi giriş yolunu kullanacağına karar vermesi için açık olan giriş yolları.
    /// </summary>
    [HttpGet("[action]")]
    [AllowAnonymous]
    public IActionResult GetLoginOptions([FromServices] IOptions<LoginSettings> loginSettings)
    {
        return Ok(new
        {
            loginSettings.Value.FormLoginEnabled,
            loginSettings.Value.WindowsLoginEnabled
        });
    }

    private string GetClientIpAddress() =>
        Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim()
            ?? HttpContext.Connection.RemoteIpAddress?.ToString();


}
