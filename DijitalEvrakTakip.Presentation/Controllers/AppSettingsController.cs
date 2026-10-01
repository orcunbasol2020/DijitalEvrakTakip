using DijitalEvrakTakip.Application.Features.AppSettingFeatures.Commands.UpdateAppSetting;
using DijitalEvrakTakip.Application.Features.AppSettingFeatures.Queries.GetAllAppSetting;
using DijitalEvrakTakip.Application.Features.AppSettingFeatures.Queries.GetAppInfo;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class AppSettingsController : ApiController
{
    public AppSettingsController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Versiyon, build tarihi ve destek bilgilerini tek seferde döner ("Hakkında" ekranı için).
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAppInfo(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAppInfoQuery(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Yönetilebilir tüm ayarları açıklamalarıyla döner (admin ekranı için).
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllAppSettingQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Update(
        UpdateAppSettingCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}
