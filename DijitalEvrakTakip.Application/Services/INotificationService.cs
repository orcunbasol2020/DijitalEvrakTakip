using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

public interface INotificationService
{
    // take boşsa tüm kayıtlar döner; en yeni başta
    Task<IList<NotificationDto>> GetByUserIdAsync(
        Guid userId,
        bool onlyUnread,
        int? take,
        CancellationToken cancellationToken);

    Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken);

    // Bildirim bulunamazsa veya kullanıcıya ait değilse false
    Task<bool> MarkAsReadAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken);

    Task<int> MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
