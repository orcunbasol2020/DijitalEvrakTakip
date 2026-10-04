using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Queries.GetNotificationsPagedByUserId;

// page 1'den başlar; pageSize boşsa 20, en fazla 100
public sealed record GetNotificationsPagedByUserIdQuery(
    Guid UserId,
    bool OnlyUnread,
    int? Page,
    int? PageSize,
    string? Search,
    IReadOnlyCollection<int>? ExcludeTypes)
    : IRequest<NotificationPagedResultDto>;
