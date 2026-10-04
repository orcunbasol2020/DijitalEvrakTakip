using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

public interface INotificationService
{
    // take boşsa tüm kayıtlar döner; en yeni başta
    Task<IList<NotificationDto>> GetByUserIdAsync(
        Guid userId,
        bool onlyUnread,
        int? take,
        string? search,
        IReadOnlyCollection<int>? excludeTypes,
        CancellationToken cancellationToken);

    // search başlık ve mesajda Türkçe büyük/küçük harf duyarsız arar
    Task<NotificationPagedResultDto> GetPagedByUserIdAsync(
        Guid userId,
        bool onlyUnread,
        int? page,
        int? pageSize,
        string? search,
        IReadOnlyCollection<int>? excludeTypes,
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
