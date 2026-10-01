using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetNotificationsByUserId;

// take boşsa tüm bildirimler döner; en yeni başta
public sealed record GetNotificationsByUserIdQuery(Guid UserId, bool OnlyUnread, int? Take)
    : IRequest<IList<NotificationDto>>;
