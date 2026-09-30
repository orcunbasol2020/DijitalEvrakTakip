using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Commands.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler
    : IRequestHandler<MarkNotificationAsReadCommand, MessageResponse>
{
    private readonly INotificationService _notificationService;

    public MarkNotificationAsReadCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<MessageResponse> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            return new MessageResponse("Geçersiz UserId");

        return await _notificationService.MarkAsReadAsync(request.Id, userId, cancellationToken)
            ? new MessageResponse("Bildirim okundu olarak işaretlendi")
            : new MessageResponse("Bildirim bulunamadı");
    }
}
