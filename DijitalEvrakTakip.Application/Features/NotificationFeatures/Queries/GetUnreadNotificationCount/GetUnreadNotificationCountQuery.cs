using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetUnreadNotificationCount;

public sealed record GetUnreadNotificationCountQuery(Guid UserId)
    : IRequest<int>;
