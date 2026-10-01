using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Commands.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsAsReadCommand, MessageResponse>
{
    private readonly INotificationService _notificationService;

    public MarkAllNotificationsAsReadCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<MessageResponse> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            return new MessageResponse("Geçersiz UserId");

        var count = await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);

        return new MessageResponse($"{count} bildirim okundu olarak işaretlendi");
    }
}
