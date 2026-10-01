using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IList<NotificationDto>> GetByUserIdAsync(
        Guid userId,
        bool onlyUnread,
        int? take,
        CancellationToken cancellationToken)
    {
        var query = _notificationRepository
            .GetAll()
            .Where(x => x.UserId == userId && !x.IsDeleted);

        if (onlyUnread)
            query = query.Where(x => !x.IsRead);

        query = query.OrderByDescending(x => x.CreatedDate);

        if (take is > 0)
            query = query.Take(take.Value);

        return await query
            .Select(x => new NotificationDto
            {
                Id = x.Id,
                Type = x.Type,
                Title = x.Title,
                Message = x.Message,
                RelatedEntityId = x.RelatedEntityId,
                IncomingDocumentId = x.IncomingDocumentId,
                IsRead = x.IsRead,
                ReadDate = x.ReadDate,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _notificationRepository
            .GetAll()
            .CountAsync(x => x.UserId == userId && !x.IsRead && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> MarkAsReadAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository
            .GetByExpressionAsync(x => x.Id == id && x.UserId == userId && !x.IsDeleted, cancellationToken);

        if (notification is null)
            return false;

        if (notification.IsRead)
            return true;

        notification.IsRead = true;
        notification.ReadDate = DateTime.UtcNow;
        _notificationRepository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<int> MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository
            .GetAll()
            .Where(x => x.UserId == userId && !x.IsRead && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        if (notifications.Count == 0)
            return 0;

        var now = DateTime.UtcNow;
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadDate = now;
            _notificationRepository.Update(notification);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return notifications.Count;
    }
}
