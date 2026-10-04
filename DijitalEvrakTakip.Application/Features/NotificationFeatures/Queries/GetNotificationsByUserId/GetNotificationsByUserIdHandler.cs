using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetNotificationsByUserId;

public sealed class GetNotificationsByUserIdHandler
    : IRequestHandler<GetNotificationsByUserIdQuery, IList<NotificationDto>>
{
    private readonly INotificationService _notificationService;

    public GetNotificationsByUserIdHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<IList<NotificationDto>> Handle(
        GetNotificationsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _notificationService.GetByUserIdAsync(
            request.UserId,
            request.OnlyUnread,
            request.Take,
            request.Search,
            request.ExcludeTypes,
            cancellationToken);
    }
}
