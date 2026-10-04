using DijitalEvrakTakip.Application.Features.NotificationFeatures.Commands.MarkAllNotificationsAsRead;
using DijitalEvrakTakip.Application.Features.NotificationFeatures.Commands.MarkNotificationAsRead;
using DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetNotificationsByUserId;
using DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetNotificationsPagedByUserId;
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
    /// Kullanıcının bildirimleri, en yeni başta.
    /// page veya pageSize gönderilirse sayfa nesnesi (items, totalCount, unreadCount, page, pageSize) döner;
    /// gönderilmezse eskisi gibi dizi döner ve take boşsa tümü gelir.
    /// search başlık ve mesajda arar; excludeTypes virgülle ayrılmış bildirim türleridir (örn. 1,2).
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> GetByUserId(
        Guid userId,
        bool onlyUnread,
        int? take,
        int? page,
        int? pageSize,
        string? search,
        string? excludeTypes,
        CancellationToken cancellationToken)
    {
        if (!TryParseTypes(excludeTypes, out var excludedTypes))
            return BadRequest(new { Message = "excludeTypes virgülle ayrılmış sayılardan oluşmalıdır (örn. 1,2)." });

        if (page is not null || pageSize is not null)
        {
            var pagedResponse = await _mediator.Send(
                new GetNotificationsPagedByUserIdQuery(userId, onlyUnread, page, pageSize, search, excludedTypes),
                cancellationToken);

            return Ok(pagedResponse);
        }

        var response = await _mediator.Send(
            new GetNotificationsByUserIdQuery(userId, onlyUnread, take, search, excludedTypes),
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

    // "1,2" → [1, 2]; boş değer filtre yok demektir
    private static bool TryParseTypes(string? value, out int[] types)
    {
        types = Array.Empty<int>();
        if (string.IsNullOrWhiteSpace(value))
            return true;

        var parsed = new List<int>();
        foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!int.TryParse(part, out int type))
                return false;
            parsed.Add(type);
        }

        types = parsed.Distinct().ToArray();
        return true;
    }
}
