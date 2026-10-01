using DijitalEvrakTakip.Application.Features.NotificationFeatures.Commands.MarkAllNotificationsAsRead;
using DijitalEvrakTakip.Application.Features.NotificationFeatures.Commands.MarkNotificationAsRead;
using DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetNotificationsByUserId;
using DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetUnreadNotificationCount;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

/// <summary>
/// Kurum içi kullanıcıların uygulama içi bildirimleri.
/// </summary>
public sealed class NotificationsController : ApiController
{
    public NotificationsController(IMediator mediator)
        : base(mediator) { }

    /// <summary>
    /// Kullanıcının bildirimleri, en yeni başta. take boşsa tümü döner.
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetByUserId(
        Guid userId,
        bool onlyUnread,
        int? take,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetNotificationsByUserIdQuery(userId, onlyUnread, take),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetUnreadCount(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var count = await _mediator.Send(
            new GetUnreadNotificationCountQuery(userId),
            cancellationToken);

        return Ok(new { UserId = userId, UnreadCount = count });
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> MarkAsRead(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response =
            await _mediator.Send(request with { UserId = ResolveUserId(request.UserId) }, cancellationToken);

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> MarkAllAsRead(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response =
            await _mediator.Send(request with { UserId = ResolveUserId(request.UserId) }, cancellationToken);

        return Ok(response);
    }
}
