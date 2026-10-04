using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class NotificationService : INotificationService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;
    private const string TurkishCollation = "Turkish_CI_AS";

    // Bu türler bildirim olarak değil bekleyen onay listesinde gösterildiği için
    // sayfalı listedeki okunmamış sayısına katılmaz
    private static readonly int[] PendingApprovalTypes =
    {
        (int)NotificationTypeEnum.ZimmetOnayTalebi,
        (int)NotificationTypeEnum.ZimmetOnayHatirlatma
    };

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
        string? search,
        IReadOnlyCollection<int>? excludeTypes,
        CancellationToken cancellationToken)
    {
        var query = BuildFilteredQuery(userId, onlyUnread, search, excludeTypes);

        if (take is > 0)
            query = query.Take(take.Value);

        return await ToDto(query).ToListAsync(cancellationToken);
    }

    public async Task<NotificationPagedResultDto> GetPagedByUserIdAsync(
        Guid userId,
        bool onlyUnread,
        int? page,
        int? pageSize,
        string? search,
        IReadOnlyCollection<int>? excludeTypes,
        CancellationToken cancellationToken)
    {
        int effectivePageSize = pageSize is null or <= 0
            ? DefaultPageSize
            : Math.Min(pageSize.Value, MaxPageSize);
        int effectivePage = page is null or <= 0 ? 1 : page.Value;

        var query = BuildFilteredQuery(userId, onlyUnread, search, excludeTypes);

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await ToDto(query
                .Skip((effectivePage - 1) * effectivePageSize)
                .Take(effectivePageSize))
            .ToListAsync(cancellationToken);

        // Zil rozeti için; arama ve tür filtresinden bağımsız
        int unreadCount = await _notificationRepository
            .GetAll()
            .CountAsync(x => x.UserId == userId
                             && !x.IsRead
                             && !x.IsDeleted
                             && !PendingApprovalTypes.Contains(x.Type),
                cancellationToken);

        return new NotificationPagedResultDto
        {
            Items = items,
            TotalCount = totalCount,
            UnreadCount = unreadCount,
            Page = effectivePage,
            PageSize = effectivePageSize
        };
    }

    // Sıralama: en yeni başta; aynı tarihli kayıtlar sayfalar arasında kaymasın diye Id ile
    private IQueryable<Notification> BuildFilteredQuery(
        Guid userId,
        bool onlyUnread,
        string? search,
        IReadOnlyCollection<int>? excludeTypes)
    {
        var query = _notificationRepository
            .GetAll()
            .Where(x => x.UserId == userId && !x.IsDeleted);

        if (onlyUnread)
            query = query.Where(x => !x.IsRead);

        if (excludeTypes is { Count: > 0 })
            query = query.Where(x => !excludeTypes.Contains(x.Type));

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Turkish_CI_AS: I/ı ve İ/i eşleşmesi Türkçe kurallarıyla yapılır
            string term = search.Trim();
            query = query.Where(x =>
                EF.Functions.Collate(x.Title, TurkishCollation).Contains(term)
                || EF.Functions.Collate(x.Message, TurkishCollation).Contains(term));
        }

        return query
            .OrderByDescending(x => x.CreatedDate)
            .ThenByDescending(x => x.Id);
    }

    private static IQueryable<NotificationDto> ToDto(IQueryable<Notification> query)
    {
        return query
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
            });
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
