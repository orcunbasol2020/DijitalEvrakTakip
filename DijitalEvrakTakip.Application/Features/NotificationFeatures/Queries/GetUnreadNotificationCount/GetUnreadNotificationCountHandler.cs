using DijitalEvrakTakip.Application.Services;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetUnreadNotificationCount;

public sealed class GetUnreadNotificationCountHandler
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    private readonly INotificationService _notificationService;

    public GetUnreadNotificationCountHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<int> Handle(
        GetUnreadNotificationCountQuery request,
        CancellationToken cancellationToken)
    {
        return _notificationService.GetUnreadCountAsync(request.UserId, cancellationToken);
    }
}
