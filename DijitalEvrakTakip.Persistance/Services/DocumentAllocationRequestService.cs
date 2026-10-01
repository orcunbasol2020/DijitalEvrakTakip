using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class DocumentAllocationRequestService : IDocumentAllocationRequestService
{
    private const int DefaultReminderIntervalHours = 4;
    private const int DefaultEscalateAfter = 3;
    private const int DefaultWorkStartHour = 8;
    private const int DefaultWorkEndHour = 18;
    private const string NoLabel = "(numarasız)";
    private const int MaxLabelLength = 150;
    private const int MaxNotificationMessageLength = 1000;

    // Alıcıda okunmamış halde tek bir güncel kaydı tutulan "onay bekliyor" bildirim türleri
    private static readonly int[] PendingNotificationTypes =
    {
        (int)NotificationTypeEnum.ZimmetOnayTalebi,
        (int)NotificationTypeEnum.ZimmetOnayHatirlatma
    };

    private readonly IDocumentAllocationRequestRepository _requestRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IDocumentTransactionRepository _transactionRepository;
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDocumentAllocationService _allocationService;
    private readonly IAppSettingService _appSettingService;
    private readonly AppDbContext _context;

    public DocumentAllocationRequestService(
        IDocumentAllocationRequestRepository requestRepository,
        INotificationRepository notificationRepository,
        IDocumentTransactionRepository transactionRepository,
        IIncomingDocumentRepository incomingDocumentRepository,
        IUserRepository userRepository,
        IDocumentAllocationService allocationService,
        IAppSettingService appSettingService,
        AppDbContext context)
    {
        _requestRepository = requestRepository;
        _notificationRepository = notificationRepository;
        _transactionRepository = transactionRepository;
        _incomingDocumentRepository = incomingDocumentRepository;
        _userRepository = userRepository;
        _allocationService = allocationService;
        _appSettingService = appSettingService;
        _context = context;
    }

    // Toplu işlemlerde aynı context'te art arda kayıt yapılır. Başarılı kayıttan sonra da
    // takip temizlenir; böylece bir sonraki talepte aynı bildirim/evrak yeniden yüklenip güncellenebilir.
    // Kaydedilemeyen değişiklikler de temizlenir, yoksa sonraki kayıtta tekrar yazılmaya çalışılır.
    private async Task<bool> TrySaveAsync(
        Func<DbUpdateException, bool> isExpectedConflict,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            _context.ChangeTracker.Clear();
            return true;
        }
        catch (DbUpdateException ex) when (isExpectedConflict(ex))
        {
            _context.ChangeTracker.Clear();
            return false;
        }
    }

    private async Task<AllocationRequestActionResultEnum> SaveActionAsync(CancellationToken cancellationToken) =>
        await TrySaveAsync(IsConcurrencyConflict, cancellationToken)
            ? AllocationRequestActionResultEnum.Basarili
            : AllocationRequestActionResultEnum.Cakisma;

    private static bool IsConcurrencyConflict(DbUpdateException ex) =>
        ex is DbUpdateConcurrencyException;

    // Filtreli unique index: evrakta ikinci bekleyen talep (SQL Server 2601 / 2627)
    private static bool IsDuplicatePendingRequest(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: 2601 or 2627 } sqlException &&
        sqlException.Message.Contains("UX_DocumentAllocationRequests_IncomingDocumentId_Pending");

    public async Task<bool> IsApprovalRequiredAsync(CancellationToken cancellationToken)
    {
        var value = await _appSettingService.GetValueAsync(AppSettingKeys.ZimmetApprovalRequired, cancellationToken);

        // Ayar yoksa veya okunamıyorsa onay istenir
        return !bool.TryParse(value, out var required) || required;
    }

    public Task<DocumentAllocationRequest?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _requestRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<DocumentAllocationRequestDto?> GetPendingDtoByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        var requests = await _requestRepository
            .GetAll()
            .Where(x => x.IncomingDocumentId == incomingDocumentId
                        && x.Status == (int)AllocationRequestStatusEnum.Beklemede
                        && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        return (await ToDtosAsync(requests, cancellationToken)).FirstOrDefault();
    }

    public async Task<bool> CreateAsync(
        DocumentAllocationRequest request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        request.Status = (int)AllocationRequestStatusEnum.Beklemede;
        request.NextReminderDate = now.Add(await GetReminderIntervalAsync(cancellationToken));

        await _requestRepository.AddAsync(request, cancellationToken);

        await _transactionRepository.AddAsync(new DocumentTransaction
        {
            DocumentId = request.IncomingDocumentId,
            TransactionType = (int)TransactionTypeEnum.ZimmetTalebi,
            UserId = request.ToUserId.ToString(),
            CreatedUserId = request.RequestedByUserId.ToString(),
            IsActive = true,
            CreatedDate = now
        }, cancellationToken);

        var documentLabel = await GetDocumentLabelAsync(request.IncomingDocumentId, cancellationToken);
        var requesterName = await GetFullNameAsync(request.RequestedByUserId, cancellationToken);
        var detail = $"{requesterName}, {documentLabel} evrakını size {GetOperationName(request.RequestedAllocationStatus)}.";

        // Yeni talep henüz kaydedilmediği için sayıya eklenir
        var pendingCount = await CountPendingAsync(request.ToUserId, null, cancellationToken) + 1;

        // Alıcıya talep başına değil, bekleyen tüm talepleri özetleyen tek bildirim
        if (pendingCount == 1)
            await ReplacePendingNotificationAsync(
                request.ToUserId,
                NotificationTypeEnum.ZimmetOnayTalebi,
                "Zimmet onayınız bekleniyor",
                $"{detail} Zimmetin üzerinize geçmesi için onay vermeniz gerekiyor.",
                request,
                cancellationToken);
        else
            await ReplacePendingNotificationAsync(
                request.ToUserId,
                NotificationTypeEnum.ZimmetOnayTalebi,
                "Onay bekleyen zimmetleriniz var",
                $"{pendingCount} evrak zimmet onayınızı bekliyor. Son gelen: {detail}",
                null,
                cancellationToken);

        return await TrySaveAsync(IsDuplicatePendingRequest, cancellationToken);
    }

    public async Task<AllocationRequestActionResultEnum> ApproveAsync(
        DocumentAllocationRequest request,
        bool notifySender,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var activeAllocation = await _allocationService
            .GetActiveByDocumentIdAsync(request.IncomingDocumentId, cancellationToken);

        // Talep beklerken zimmet başka bir işlemle değiştiyse onay eski durumu ezmemeli
        if (activeAllocation?.Id != request.FromAllocationId)
        {
            await StageInvalidateAsync(request, now, cancellationToken);
            return await TrySaveAsync(IsConcurrencyConflict, cancellationToken)
                ? AllocationRequestActionResultEnum.Gecersiz
                : AllocationRequestActionResultEnum.Cakisma;
        }

        var isTeslim = request.RequestedAllocationStatus == (int)AllocationStatusEnum.Teslim;

        var allocation = new DocumentAllocation
        {
            IncomingDocumentId = request.IncomingDocumentId,
            UserId = request.ToUserId,
            UserType = (int)AllocationUserTypeEnum.Internal,
            CreatedUserId = request.RequestedByUserId,
            Status = isTeslim
                ? (int)AllocationStatusEnum.TeslimAlindi
                : (int)AllocationStatusEnum.DevirAlindi,
            Source = (int)AllocationSourceEnum.EvrakTakip,
            IsActive = true
        };

        await _allocationService.StageCreateAsync(
            allocation,
            activeAllocation,
            isTeslim ? (int)TransactionTypeEnum.TeslimAlindi : (int)TransactionTypeEnum.DevirAlindi,
            request.ToUserId,
            cancellationToken);

        request.Status = (int)AllocationRequestStatusEnum.Onaylandi;
        request.RespondedDate = now;
        request.RespondedUserId = request.ToUserId;
        request.ResultAllocationId = allocation.Id;
        request.NextReminderDate = null;
        _requestRepository.Update(request);

        await RefreshPendingNotificationAfterResolveAsync(request, now, cancellationToken);

        if (notifySender)
            await StageResultNotificationsAsync(
                NotificationTypeEnum.ZimmetOnaylandi,
                new[] { request },
                request.ToUserId,
                null,
                cancellationToken);

        // Zimmet, transaction, evrak durumu, talep ve bildirimler tek commit
        return await SaveActionAsync(cancellationToken);
    }

    public async Task<AllocationRequestActionResultEnum> RejectAsync(
        DocumentAllocationRequest request,
        string? note,
        bool notifySender,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        request.Status = (int)AllocationRequestStatusEnum.Reddedildi;
        request.RespondedDate = now;
        request.RespondedUserId = request.ToUserId;
        request.ResponseNote = note;
        request.NextReminderDate = null;
        _requestRepository.Update(request);

        await _transactionRepository.AddAsync(new DocumentTransaction
        {
            DocumentId = request.IncomingDocumentId,
            TransactionType = (int)TransactionTypeEnum.ZimmetTalebiReddedildi,
            UserId = request.ToUserId.ToString(),
            CreatedUserId = request.ToUserId.ToString(),
            IsActive = true,
            CreatedDate = now
        }, cancellationToken);

        await RefreshPendingNotificationAfterResolveAsync(request, now, cancellationToken);

        if (notifySender)
            await StageResultNotificationsAsync(
                NotificationTypeEnum.ZimmetReddedildi,
                new[] { request },
                request.ToUserId,
                note,
                cancellationToken);

        return await SaveActionAsync(cancellationToken);
    }

    public async Task<AllocationRequestActionResultEnum> CancelAsync(
        DocumentAllocationRequest request,
        Guid cancelledByUserId,
        string? note,
        bool notifyReceiver,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        request.Status = (int)AllocationRequestStatusEnum.IptalEdildi;
        request.RespondedDate = now;
        request.RespondedUserId = cancelledByUserId;
        request.ResponseNote = note;
        request.NextReminderDate = null;
        _requestRepository.Update(request);

        await _transactionRepository.AddAsync(new DocumentTransaction
        {
            DocumentId = request.IncomingDocumentId,
            TransactionType = (int)TransactionTypeEnum.ZimmetTalebiIptal,
            UserId = request.ToUserId.ToString(),
            CreatedUserId = cancelledByUserId.ToString(),
            IsActive = true,
            CreatedDate = now
        }, cancellationToken);

        await RefreshPendingNotificationAfterResolveAsync(request, now, cancellationToken);

        if (notifyReceiver)
            await StageResultNotificationsAsync(
                NotificationTypeEnum.ZimmetTalebiIptal,
                new[] { request },
                cancelledByUserId,
                note,
                cancellationToken);

        return await SaveActionAsync(cancellationToken);
    }

    public async Task NotifyBulkResultAsync(
        NotificationTypeEnum type,
        IList<DocumentAllocationRequest> requests,
        Guid actorUserId,
        string? note,
        CancellationToken cancellationToken)
    {
        if (requests.Count == 0)
            return;

        await StageResultNotificationsAsync(type, requests, actorUserId, note, cancellationToken);

        // Bildirim kaydedilemese de onay / red / iptal zaten kaydedildi; yalnızca bildirimler yazılır
        await TrySaveAsync(_ => false, cancellationToken);
    }

    public async Task<IList<DocumentAllocationRequestDto>> GetPendingByToUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var requests = await _requestRepository
            .GetAll()
            .Where(x => x.ToUserId == userId
                        && x.Status == (int)AllocationRequestStatusEnum.Beklemede
                        && !x.IsDeleted)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return await ToDtosAsync(requests, cancellationToken);
    }

    public async Task<IList<DocumentAllocationRequestDto>> GetSentByUserIdAsync(
        Guid userId,
        bool onlyPending,
        CancellationToken cancellationToken)
    {
        var query = _requestRepository
            .GetAll()
            .Where(x => (x.FromUserId == userId || x.RequestedByUserId == userId) && !x.IsDeleted);

        if (onlyPending)
            query = query.Where(x => x.Status == (int)AllocationRequestStatusEnum.Beklemede);

        var requests = await query
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return await ToDtosAsync(requests, cancellationToken);
    }

    public async Task<IList<DocumentAllocationRequestDto>> GetByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        var requests = await _requestRepository
            .GetAll()
            .Where(x => x.IncomingDocumentId == incomingDocumentId && !x.IsDeleted)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return await ToDtosAsync(requests, cancellationToken);
    }

    public async Task<int> SendRemindersAsync(CancellationToken cancellationToken)
    {
        if (!await GetBoolSettingAsync(AppSettingKeys.ZimmetReminderEnabled, true, cancellationToken))
            return 0;

        // Hatırlatmalar yalnızca hafta içi mesai saatlerinde (sunucu saati) gönderilir
        var localNow = DateTime.Now;
        var workStart = await GetIntSettingAsync(AppSettingKeys.ZimmetReminderWorkStartHour, DefaultWorkStartHour, cancellationToken);
        var workEnd = await GetIntSettingAsync(AppSettingKeys.ZimmetReminderWorkEndHour, DefaultWorkEndHour, cancellationToken);

        if (localNow.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ||
            localNow.Hour < workStart || localNow.Hour >= workEnd)
            return 0;

        var now = DateTime.UtcNow;

        var dueRequests = await _requestRepository
            .GetAll()
            .Where(x => x.Status == (int)AllocationRequestStatusEnum.Beklemede
                        && !x.IsDeleted
                        && x.NextReminderDate != null
                        && x.NextReminderDate <= now)
            .ToListAsync(cancellationToken);

        if (dueRequests.Count == 0)
            return 0;

        var interval = await GetReminderIntervalAsync(cancellationToken);
        var escalateAfter = await GetIntSettingAsync(AppSettingKeys.ZimmetReminderEscalateAfter, DefaultEscalateAfter, cancellationToken);

        // Bu arada zimmeti değişmiş talepler hatırlatılmaz, geçersiz yapılır
        var documentIds = dueRequests.Select(x => x.IncomingDocumentId).Distinct().ToList();
        var activeAllocationIds = await GetActiveAllocationIdsAsync(documentIds, cancellationToken);
        var documentLabels = await GetDocumentLabelsAsync(documentIds, cancellationToken);

        var validRequests = new List<DocumentAllocationRequest>();
        var invalidatedIds = new List<Guid>();
        foreach (var request in dueRequests)
        {
            activeAllocationIds.TryGetValue(request.IncomingDocumentId, out var activeAllocationId);

            if (activeAllocationId != request.FromAllocationId)
            {
                await StageInvalidateAsync(request, now, cancellationToken);
                invalidatedIds.Add(request.Id);
                continue;
            }

            request.ReminderCount++;
            request.LastReminderDate = now;
            request.NextReminderDate = now.Add(interval);
            _requestRepository.Update(request);
            validRequests.Add(request);
        }

        // Alıcıya evrak başına değil, kişi başına tek hatırlatma; önceki okunmamış hatırlatmanın yerine geçer
        foreach (var group in validRequests.GroupBy(x => x.ToUserId))
        {
            var pendingCount = await CountPendingAsync(group.Key, invalidatedIds, cancellationToken);
            var single = pendingCount == 1 ? group.First() : null;

            await ReplacePendingNotificationAsync(
                group.Key,
                NotificationTypeEnum.ZimmetOnayHatirlatma,
                "Onay bekleyen zimmetleriniz var",
                single is not null
                    ? $"{documentLabels[single.IncomingDocumentId]} evrakı zimmet onayınızı bekliyor."
                    : $"{pendingCount} evrak zimmet onayınızı bekliyor.",
                single,
                cancellationToken);
        }

        // Eşiğe ulaşan talepler için devredene ve işlemi yapana tek seferlik gecikme bildirimi
        var escalated = validRequests
            .Where(x => escalateAfter > 0 && x.ReminderCount == escalateAfter)
            .ToList();

        if (escalated.Count > 0)
        {
            var toUserNames = await GetFullNamesAsync(escalated.Select(x => x.ToUserId), cancellationToken);

            var bySender = escalated
                .SelectMany(x => GetSenderIds(x).Select(senderId => (SenderId: senderId, Request: x)))
                .GroupBy(x => x.SenderId);

            foreach (var group in bySender)
            {
                var items = group.Select(x => x.Request).ToList();
                var message = items.Count == 1
                    ? $"{toUserNames.GetValueOrDefault(items[0].ToUserId)}, {escalateAfter} hatırlatmaya rağmen {documentLabels[items[0].IncomingDocumentId]} evrakının zimmetini henüz onaylamadı."
                    : $"{items.Count} evrak için gönderdiğiniz zimmet talebi {escalateAfter} hatırlatmaya rağmen henüz onaylanmadı.";

                await AddNotificationAsync(
                    group.Key,
                    NotificationTypeEnum.ZimmetOnayGecikme,
                    "Zimmet onayı gecikiyor",
                    message,
                    items.Count == 1 ? items[0] : null,
                    cancellationToken);
            }
        }

        // Kullanıcı aynı anda onay verdiyse bu tur kaydedilmez; sonraki turda tekrar denenir
        if (!await TrySaveAsync(IsConcurrencyConflict, cancellationToken))
            return 0;

        return validRequests.Count;
    }

    private async Task StageInvalidateAsync(
        DocumentAllocationRequest request,
        DateTime now,
        CancellationToken cancellationToken)
    {
        request.Status = (int)AllocationRequestStatusEnum.Gecersiz;
        request.RespondedDate = now;
        request.ResponseNote = "Talep beklerken evrağın zimmeti başka bir işlemle değişti";
        request.NextReminderDate = null;
        _requestRepository.Update(request);

        await RefreshPendingNotificationAfterResolveAsync(request, now, cancellationToken);

        var documentLabel = await GetDocumentLabelAsync(request.IncomingDocumentId, cancellationToken);

        foreach (var userId in GetSenderIds(request).Append(request.ToUserId).Distinct())
        {
            await AddNotificationAsync(
                userId,
                NotificationTypeEnum.ZimmetTalebiGecersiz,
                "Zimmet talebi geçersiz oldu",
                $"{documentLabel} evrakının zimmeti talep beklerken değiştiği için zimmet talebi geçersiz sayıldı.",
                request,
                cancellationToken);
        }
    }

    // Onay / red bildirimi devredene ve işlemi yapana, iptal bildirimi alıcıya gider.
    // Aynı kişiye giden sonuçlar tek bildirimde toplanır.
    private async Task StageResultNotificationsAsync(
        NotificationTypeEnum type,
        IList<DocumentAllocationRequest> requests,
        Guid actorUserId,
        string? note,
        CancellationToken cancellationToken)
    {
        var actorName = await GetFullNameAsync(actorUserId, cancellationToken);
        var documentLabels = await GetDocumentLabelsAsync(requests.Select(x => x.IncomingDocumentId), cancellationToken);
        var reason = string.IsNullOrWhiteSpace(note) ? string.Empty : $" Gerekçe: {note}";

        var byRecipient = type == NotificationTypeEnum.ZimmetTalebiIptal
            ? requests.Select(x => (UserId: x.ToUserId, Request: x))
            : requests.SelectMany(x => GetSenderIds(x).Select(userId => (UserId: userId, Request: x)));

        foreach (var group in byRecipient.Where(x => x.UserId != actorUserId).GroupBy(x => x.UserId))
        {
            var items = group.Select(x => x.Request).ToList();
            var single = items.Count == 1 ? items[0] : null;
            var subject = single is not null
                ? $"{documentLabels[single.IncomingDocumentId]} evrakının"
                : $"{items.Count} evrakın";

            var (title, message) = type switch
            {
                NotificationTypeEnum.ZimmetOnaylandi => (
                    "Zimmet onaylandı",
                    $"{actorName}, {subject} zimmetini kabul etti."),
                NotificationTypeEnum.ZimmetReddedildi => (
                    "Zimmet reddedildi",
                    $"{actorName}, {subject} zimmetini kabul etmedi. {(single is not null ? "Evrak" : "Evraklar")} üzerinizde kalmaya devam ediyor.{reason}"),
                _ => (
                    "Zimmet talebi geri çekildi",
                    single is not null
                        ? $"{actorName}, {documentLabels[single.IncomingDocumentId]} evrakı için size gönderdiği zimmet talebini geri çekti.{reason}"
                        : $"{actorName}, {items.Count} evrak için size gönderdiği zimmet talebini geri çekti.{reason}")
            };

            await AddNotificationAsync(group.Key, type, title, message, single, cancellationToken);
        }
    }

    // Devreden ve işlemi yapan kullanıcı (alıcı hariç)
    private static IEnumerable<Guid> GetSenderIds(DocumentAllocationRequest request)
    {
        var ids = new List<Guid> { request.RequestedByUserId };
        if (request.FromUserId is Guid fromUserId)
            ids.Add(fromUserId);

        return ids.Where(x => x != request.ToUserId).Distinct();
    }

    private Task AddNotificationAsync(
        Guid userId,
        NotificationTypeEnum type,
        string title,
        string message,
        DocumentAllocationRequest? request,
        CancellationToken cancellationToken)
    {
        return _notificationRepository.AddAsync(new Notification
        {
            UserId = userId,
            Type = (int)type,
            Title = title,
            Message = Truncate(message, MaxNotificationMessageLength),
            RelatedEntityId = request?.Id,
            IncomingDocumentId = request?.IncomingDocumentId
        }, cancellationToken);
    }

    // Alıcının okunmamış onay talebi / hatırlatma bildirimleri kaldırılır, yerine tek güncel bildirim eklenir
    private async Task ReplacePendingNotificationAsync(
        Guid toUserId,
        NotificationTypeEnum type,
        string title,
        string message,
        DocumentAllocationRequest? singleRequest,
        CancellationToken cancellationToken)
    {
        foreach (var notification in await GetUnreadPendingNotificationsAsync(toUserId, cancellationToken))
            notification.IsDeleted = true;

        await AddNotificationAsync(toUserId, type, title, message, singleRequest, cancellationToken);
    }

    // Talep sonuçlanınca alıcının okunmamış "onay bekliyor" bildirimi kalan talep sayısına göre güncellenir;
    // bekleyen talep kalmadıysa veya bildirim yalnızca bu talebe aitse okundu sayılır
    private async Task RefreshPendingNotificationAfterResolveAsync(
        DocumentAllocationRequest request,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var notifications = await GetUnreadPendingNotificationsAsync(request.ToUserId, cancellationToken);
        if (notifications.Count == 0)
            return;

        var remaining = await CountPendingAsync(request.ToUserId, new[] { request.Id }, cancellationToken);

        foreach (var notification in notifications)
        {
            if (remaining == 0 || notification.RelatedEntityId == request.Id)
            {
                notification.IsRead = true;
                notification.ReadDate = now;
            }
            else if (notification.RelatedEntityId is null)
            {
                notification.Message = $"{remaining} evrak zimmet onayınızı bekliyor.";
            }
        }
    }

    // Değiştirileceği için takipli yüklenir
    private Task<List<Notification>> GetUnreadPendingNotificationsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _notificationRepository
            .GetAllWithTracking()
            .Where(x => x.UserId == userId
                        && PendingNotificationTypes.Contains(x.Type)
                        && !x.IsRead
                        && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    private Task<int> CountPendingAsync(
        Guid toUserId,
        IEnumerable<Guid>? excludedRequestIds,
        CancellationToken cancellationToken)
    {
        var excluded = excludedRequestIds?.ToList() ?? new List<Guid>();

        return _requestRepository
            .GetAll()
            .CountAsync(x => x.ToUserId == toUserId
                             && x.Status == (int)AllocationRequestStatusEnum.Beklemede
                             && !x.IsDeleted
                             && !excluded.Contains(x.Id),
                cancellationToken);
    }

    private async Task<Dictionary<Guid, Guid>> GetActiveAllocationIdsAsync(
        IList<Guid> documentIds,
        CancellationToken cancellationToken)
    {
        var allocations = new Dictionary<Guid, Guid>();
        foreach (var documentId in documentIds)
        {
            var active = await _allocationService.GetActiveByDocumentIdAsync(documentId, cancellationToken);
            if (active is not null)
                allocations[documentId] = active.Id;
        }

        return allocations;
    }

    private static string GetOperationName(int requestedAllocationStatus) =>
        requestedAllocationStatus == (int)AllocationStatusEnum.Teslim ? "teslim etti" : "devretti";

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..(maxLength - 1)] + "…";

    private async Task<string> GetDocumentLabelAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        var labels = await GetDocumentLabelsAsync(new[] { incomingDocumentId }, cancellationToken);
        return labels[incomingDocumentId];
    }

    // Bildirim metinlerinde evrak: "Sayı / Konu" (boş olanlar atlanır)
    private async Task<Dictionary<Guid, string>> GetDocumentLabelsAsync(
        IEnumerable<Guid> documentIds,
        CancellationToken cancellationToken)
    {
        var ids = documentIds.Distinct().ToList();

        var documents = await _incomingDocumentRepository
            .GetAll()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new { x.Id, x.OrginalNo, x.QrCode, x.Subject })
            .ToListAsync(cancellationToken);

        var labels = ids.ToDictionary(x => x, _ => NoLabel);

        foreach (var document in documents)
        {
            var number = !string.IsNullOrWhiteSpace(document.OrginalNo) ? document.OrginalNo : document.QrCode;
            var parts = new[] { number, document.Subject }
                .Where(x => !string.IsNullOrWhiteSpace(x));
            var label = string.Join(" / ", parts);

            labels[document.Id] = string.IsNullOrEmpty(label) ? NoLabel : $"\"{Truncate(label, MaxLabelLength)}\"";
        }

        return labels;
    }

    private async Task<string> GetFullNameAsync(Guid userId, CancellationToken cancellationToken)
    {
        var names = await GetFullNamesAsync(new[] { userId }, cancellationToken);
        return names.GetValueOrDefault(userId, "Bir kullanıcı");
    }

    private async Task<Dictionary<Guid, string>> GetFullNamesAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken)
    {
        var ids = userIds.Distinct().ToList();

        return await _userRepository
            .GetAll()
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => $"{x.Name} {x.Surname}", cancellationToken);
    }

    private async Task<IList<DocumentAllocationRequestDto>> ToDtosAsync(
        IList<DocumentAllocationRequest> requests,
        CancellationToken cancellationToken)
    {
        if (requests.Count == 0)
            return new List<DocumentAllocationRequestDto>();

        var userIds = requests
            .SelectMany(x => new[] { x.FromUserId, x.ToUserId, x.RequestedByUserId })
            .Where(x => x.HasValue)
            .Select(x => x!.Value);
        var fullNames = await GetFullNamesAsync(userIds, cancellationToken);

        var documentIds = requests.Select(x => x.IncomingDocumentId).Distinct().ToList();
        var documentsById = await _incomingDocumentRepository
            .GetAll()
            .Where(x => documentIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.OrginalNo,
                x.QrCode,
                x.DocumentName,
                x.Subject,
                x.DocumentDate
            })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        return requests
            .Select(x =>
            {
                documentsById.TryGetValue(x.IncomingDocumentId, out var document);

                return new DocumentAllocationRequestDto
                {
                    Id = x.Id,
                    IncomingDocumentId = x.IncomingDocumentId,
                    OrginalNo = document?.OrginalNo,
                    QrCode = document?.QrCode,
                    DocumentName = document?.DocumentName,
                    Subject = document?.Subject,
                    DocumentDate = document?.DocumentDate,
                    FromUserId = x.FromUserId,
                    FromUserFullName = x.FromUserId is Guid fromUserId
                        ? fullNames.GetValueOrDefault(fromUserId, string.Empty)
                        : string.Empty,
                    ToUserId = x.ToUserId,
                    ToUserFullName = fullNames.GetValueOrDefault(x.ToUserId, string.Empty),
                    RequestedByUserId = x.RequestedByUserId,
                    RequestedByFullName = fullNames.GetValueOrDefault(x.RequestedByUserId, string.Empty),
                    RequestedAllocationStatus = x.RequestedAllocationStatus,
                    Status = x.Status,
                    ResponseNote = x.ResponseNote,
                    RespondedDate = x.RespondedDate,
                    ResultAllocationId = x.ResultAllocationId,
                    ReminderCount = x.ReminderCount,
                    LastReminderDate = x.LastReminderDate,
                    CreatedDate = x.CreatedDate
                };
            })
            .ToList();
    }

    private async Task<TimeSpan> GetReminderIntervalAsync(CancellationToken cancellationToken)
    {
        var hours = await GetIntSettingAsync(AppSettingKeys.ZimmetReminderIntervalHours, DefaultReminderIntervalHours, cancellationToken);
        return TimeSpan.FromHours(hours < 1 ? 1 : hours);
    }

    private async Task<bool> GetBoolSettingAsync(string key, bool defaultValue, CancellationToken cancellationToken)
    {
        var value = await _appSettingService.GetValueAsync(key, cancellationToken);
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    private async Task<int> GetIntSettingAsync(string key, int defaultValue, CancellationToken cancellationToken)
    {
        var value = await _appSettingService.GetValueAsync(key, cancellationToken);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }
}
