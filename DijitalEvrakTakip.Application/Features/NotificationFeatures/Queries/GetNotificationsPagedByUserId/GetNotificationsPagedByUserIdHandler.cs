using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetNotificationsPagedByUserId;

public sealed class GetNotificationsPagedByUserIdHandler
    : IRequestHandler<GetNotificationsPagedByUserIdQuery, NotificationPagedResultDto>
{
    private readonly INotificationService _notificationService;

    public GetNotificationsPagedByUserIdHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<NotificationPagedResultDto> Handle(
        GetNotificationsPagedByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _notificationService.GetPagedByUserIdAsync(
            request.UserId,
            request.OnlyUnread,
            request.Page,
            request.PageSize,
            request.Search,
            request.ExcludeTypes,
            cancellationToken);
    }
}
