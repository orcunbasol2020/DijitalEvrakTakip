using DijitalEvrakTakip.Application.Features.UserLoginLogFeatures.Queries.GetUserLoginLogs;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class UserLoginLogsController : ApiController
{
    public UserLoginLogsController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Login denemelerini yeniden eskiye listeler.
    /// isSuccess: true = başarılı, false = başarısız, boş = hepsi. pageSize boşsa tüm kayıtlar döner.
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(
        string? userName,
        Guid? userId,
        bool? isSuccess,
        DateTime? startDate,
        DateTime? endDate,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        PagedResultDto<UserLoginLogDto> response = await _mediator.Send(
            new GetUserLoginLogsQuery(userName, userId, isSuccess, startDate, endDate, page, pageSize),
            cancellationToken);

        return Ok(response);
    }
}
