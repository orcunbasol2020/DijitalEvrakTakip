using System.Security.Cryptography;
using DijitalEvrakTakip.Application.Eyp;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class AtlasTransferService : IAtlasTransferService
{
    // Arka plan servisi ve elle tetikleme aynı anda çalışmasın
    private static readonly SemaphoreSlim Gate = new(1, 1);

    private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(10);

    // Bu süreden uzun Aktarılıyor'da kalan evrak (uygulama işlem sırasında kapanmış) kuyruğa geri alınır
    private static readonly TimeSpan StuckAfter = TimeSpan.FromMinutes(30);

    // n. geçici hatadan sonraki bekleme; son değer sonraki tüm denemelerde kullanılır
    private static readonly TimeSpan[] RetryDelays =
    {
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(15),
        TimeSpan.FromMinutes(60)
    };

    private const int DefaultBatchSize = 10;
    private const int MaxBatchSize = 100;
    private const int DefaultMaxTryCount = 5;
    private const int ErrorMaxLength = 2000; // EypPackages.LastError / EypTransfers.Error kolon uzunluğu
    private const int FailedListLimit = 500;

    private readonly IEypService _eypService;
    private readonly IAtlasEypTransferClient _transferClient;
    private readonly IEypPackageStorageService _packageStorage;
    private readonly IIncomingDocumentStorageService _documentStorage;
    private readonly IAppSettingService _appSettingService;
    private readonly AppDbContext _context;
    private readonly ILogger<AtlasTransferService> _logger;

    public AtlasTransferService(
        IEypService eypService,
        IAtlasEypTransferClient transferClient,
        IEypPackageStorageService packageStorage,
        IIncomingDocumentStorageService documentStorage,
        IAppSettingService appSettingService,
        AppDbContext context,
        ILogger<AtlasTransferService> logger)
    {
        _eypService = eypService;
        _transferClient = transferClient;
        _packageStorage = packageStorage;
        _documentStorage = documentStorage;
        _appSettingService = appSettingService;
        _context = context;
        _logger = logger;
    }

    public async Task<TimeSpan> GetIntervalAsync(CancellationToken cancellationToken)
    {
        var seconds = await GetIntSettingAsync(
            AppSettingKeys.AtlasTransferIntervalSeconds, (int)DefaultInterval.TotalSeconds, cancellationToken);

        var interval = TimeSpan.FromSeconds(seconds);
        return interval < MinInterval ? MinInterval : interval;
    }

    public async Task<AtlasTransferRunResultDto> RunAsync(bool ignoreEnabledFlag, CancellationToken cancellationToken)
    {
        if (!await Gate.WaitAsync(TimeSpan.Zero, cancellationToken))
            return NotRun("Atlas aktarımı şu anda zaten çalışıyor.");

        try
        {
            if (!ignoreEnabledFlag
                && !await GetBoolSettingAsync(AppSettingKeys.AtlasTransferEnabled, false, cancellationToken))
                return NotRun("Otomatik Atlas aktarımı kapalı.");

            // Ayar eksikken evraklar Hatalı'ya düşmesin; kuyrukta beklemeye devam eder
            var recipient = await GetRecipientAsync(cancellationToken);
            if (recipient is null)
                return NotRun("Evrakı alan kurumun KKK kodu veya adı ayarlarda tanımlı değil; aktarım yapılmadı.");

            var recovered = await RecoverStuckAsync(cancellationToken);

            var batchSize = Math.Min(
                await GetIntSettingAsync(AppSettingKeys.AtlasTransferBatchSize, DefaultBatchSize, cancellationToken),
                MaxBatchSize);
            var maxTryCount = await GetIntSettingAsync(
                AppSettingKeys.AtlasTransferMaxTryCount, DefaultMaxTryCount, cancellationToken);

            var documentIds = await ClaimAsync(batchSize, cancellationToken);

            int succeeded = 0, requeued = 0, failed = 0;
            foreach (var documentId in documentIds)
            {
                try
                {
                    switch (await ProcessAsync(documentId, recipient, maxTryCount, cancellationToken))
                    {
                        case TransferOutcome.Succeeded: succeeded++; break;
                        case TransferOutcome.Requeued: requeued++; break;
                        case TransferOutcome.Failed: failed++; break;
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // Sonuç kaydedilemedi; evrak Aktarılıyor'da kalır ve süre dolunca kuyruğa geri alınır
                    _logger.LogError(ex, "Atlas aktarımı sonucu kaydedilemedi: {DocumentId}", documentId);
                }
                finally
                {
                    _context.ChangeTracker.Clear();
                }
            }

            var message = documentIds.Count == 0
                ? "Kuyrukta aktarılacak evrak yok."
                : $"{documentIds.Count} evrak işlendi: {succeeded} aktarıldı, {requeued} tekrar denenecek, {failed} hatalı.";
            if (recovered > 0)
                message += $" Takılı kalan {recovered} evrak kuyruğa geri alındı.";

            return new(true, documentIds.Count, succeeded, requeued, failed, recovered, message);
        }
        finally
        {
            Gate.Release();
        }
    }

    public async Task EnqueueAsync(IncomingDocument document, CancellationToken cancellationToken)
    {
        switch ((PublishStatusEnum?)document.SubmissionStatus)
        {
            case PublishStatusEnum.Aktariliyor:
                throw new Exception("Evrak şu anda Atlas'a aktarılıyor, tekrar yayınlanamaz.");
            case PublishStatusEnum.Basarili:
                throw new Exception("Evrak Atlas'a zaten aktarıldı.");
        }

        if (document.ExternalInstitution is null && document.ExternalInstitutionId is not null)
        {
            // Silinmiş kurum sorgu filtresine takılır
            document.ExternalInstitution = await _context.Set<ExternalInstitution>()
                .FirstOrDefaultAsync(x => x.Id == document.ExternalInstitutionId, cancellationToken)
                ?? throw new Exception("Evrak Atlas'a aktarılamaz: gönderen kurum bulunamadı.");
        }

        var errors = IncomingDocumentEypMapper.Validate(document);
        if (errors.Count > 0)
            throw new Exception("Evrak Atlas'a aktarılamaz: " + string.Join(" ", errors));

        // Önceki paket evrakın eski bilgileriyle üretildi; güncel bilgilerle yeni paket üretilecek
        var previousPackages = await _context.Set<EypPackage>()
            .Where(x => x.DocumentId == document.Id && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var package in previousPackages)
            package.IsDeleted = true;

        document.SubmissionStatus = (int)PublishStatusEnum.Kuyrukta;
        document.SubmissionUpdatedAt = DateTime.UtcNow;
    }

    public async Task RetryAsync(Guid documentId, string userId, CancellationToken cancellationToken)
    {
        var document = await _context.Set<IncomingDocument>()
            .Include(x => x.ExternalInstitution)
            .FirstOrDefaultAsync(x => x.Id == documentId && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Evrak bulunamadı.");

        if (document.SubmissionStatus != (int)PublishStatusEnum.Hatali)
            throw new Exception("Yalnızca Atlas aktarımı hatalı olan evrak yeniden kuyruğa alınabilir.");

        await EnqueueAsync(document, cancellationToken);

        _context.Add(new DocumentTransaction
        {
            DocumentId = document.Id,
            TransactionType = (int)TransactionTypeEnum.Yayinla,
            UserId = userId,
            IsActive = true
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AtlasTransferStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var counts = await _context.Set<IncomingDocument>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .GroupBy(x => x.SubmissionStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int CountOf(PublishStatusEnum status) =>
            counts.Where(x => x.Status == (int)status).Sum(x => x.Count);

        return new AtlasTransferStatsDto(
            // Durumu boş olan eski kayıtlar yayınlanmamış sayılır
            NotPublished: CountOf(PublishStatusEnum.Yayinlanmadi) + counts.Where(x => x.Status == null).Sum(x => x.Count),
            Queued: CountOf(PublishStatusEnum.Kuyrukta),
            Transferring: CountOf(PublishStatusEnum.Aktariliyor),
            Succeeded: CountOf(PublishStatusEnum.Basarili),
            Failed: CountOf(PublishStatusEnum.Hatali));
    }

    public async Task<IReadOnlyList<AtlasTransferFailedDocumentDto>> GetFailedAsync(CancellationToken cancellationToken)
    {
        return await (
                from document in _context.Set<IncomingDocument>().AsNoTracking()
                where document.SubmissionStatus == (int)PublishStatusEnum.Hatali && !document.IsDeleted
                join activePackage in _context.Set<EypPackage>().Where(p => !p.IsDeleted)
                    on document.Id equals activePackage.DocumentId into packages
                from package in packages.DefaultIfEmpty()
                orderby document.SubmissionUpdatedAt descending
                select new AtlasTransferFailedDocumentDto(
                    document.Id,
                    document.QrCode,
                    document.Subject,
                    package == null ? 0 : package.TryCount,
                    package == null ? null : package.LastError,
                    package == null ? null : package.LastAttemptAt))
            .Take(FailedListLimit)
            .ToListAsync(cancellationToken);
    }

    private async Task<TransferOutcome> ProcessAsync(
        Guid documentId,
        EypRecipient recipient,
        int maxTryCount,
        CancellationToken cancellationToken)
    {
        var document = await _context.Set<IncomingDocument>()
            .Include(x => x.ExternalInstitution)
            .FirstOrDefaultAsync(x => x.Id == documentId, cancellationToken);

        if (document is null)
            return TransferOutcome.Skipped;

        var package = await _context.Set<EypPackage>()
            .FirstOrDefaultAsync(x => x.DocumentId == documentId && !x.IsDeleted, cancellationToken);

        if (package is null)
        {
            package = new EypPackage { DocumentId = documentId };
            _context.Add(package);
        }

        package.TryCount++;
        package.LastAttemptAt = DateTime.UtcNow;

        var attempt = new EypTransfer { EypPackageId = package.Id, AttemptNo = package.TryCount };
        _context.Add(attempt);

        // Paket ilk denemede üretilir; sonraki denemelerde aynı dosya gönderilir
        byte[] content;
        if (package.FileName is not null && _packageStorage.Exists(package.FileName))
        {
            content = await _packageStorage.ReadAsync(package.FileName, cancellationToken);
        }
        else
        {
            try
            {
                content = await BuildPackageAsync(document, package, recipient, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Eksik bilgi veya bozuk dosya; tekrar denemek sonucu değiştirmez
                _logger.LogWarning(ex, "EYP paketi oluşturulamadı: {DocumentId}", documentId);
                return await FinishWithFailureAsync(
                    document, package, attempt, $"EYP oluşturulamadı: {ex.Message}", permanent: true, cancellationToken);
            }
        }

        AtlasEypTransferResultDto result;
        try
        {
            result = await _transferClient.SendAsync(
                new AtlasEypSubmissionDto(document.Id, document.QrCode, package.FileName!, content),
                cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "EYP paketi Atlas'a gönderilemedi: {DocumentId}", documentId);
            result = AtlasEypTransferResultDto.TransientFailure(ex.Message);
        }

        if (result.IsSuccess)
            return await FinishWithSuccessAsync(document, package, attempt, result.AtlasReferenceId, cancellationToken);

        if (result.IsPermanentFailure)
        {
            return await FinishWithFailureAsync(
                document, package, attempt, $"Atlas paketi reddetti: {result.Message}", permanent: true, cancellationToken);
        }

        var limitReached = package.TryCount >= maxTryCount;
        var error = $"Atlas'a gönderilemedi: {result.Message}";
        if (limitReached)
            error += $" ({package.TryCount} deneme yapıldı.)";

        return await FinishWithFailureAsync(document, package, attempt, error, permanent: limitReached, cancellationToken);
    }

    private async Task<byte[]> BuildPackageAsync(
        IncomingDocument document,
        EypPackage package,
        EypRecipient recipient,
        CancellationToken cancellationToken)
    {
        var metadata = IncomingDocumentEypMapper.Map(document, recipient);

        // Dosyası yüklenmemiş (güvenlik nedeniyle yalnızca bilgileri girilmiş) evrakta standart PDF kullanılır
        byte[] ustYazi;
        bool usedPlaceholder;
        if (string.IsNullOrWhiteSpace(document.DocumentName))
        {
            ustYazi = PhysicalDeliveryPlaceholderPdf.Get();
            usedPlaceholder = true;
        }
        else
        {
            if (!_documentStorage.Exists(document.DocumentName))
                throw new InvalidOperationException($"Evrak dosyası bulunamadı: {document.DocumentName}");

            ustYazi = await File.ReadAllBytesAsync(_documentStorage.GetFullPath(document.DocumentName), cancellationToken);
            usedPlaceholder = false;
        }

        var created = await _eypService.EypOlusturAsync(metadata, ustYazi);
        var completed = await _eypService.EypTamamlaAsync(created, metadata);

        package.FileName = await _packageStorage.SaveAsync(document.QrCode, package.Id, completed, cancellationToken);
        package.FileSize = completed.Length;
        package.Sha256 = Convert.ToHexString(SHA256.HashData(completed));
        package.UsedPlaceholderContent = usedPlaceholder;

        return completed;
    }

    private async Task<TransferOutcome> FinishWithSuccessAsync(
        IncomingDocument document,
        EypPackage package,
        EypTransfer attempt,
        string? atlasReferenceId,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        attempt.IsSuccess = true;
        attempt.AtlasReferenceId = atlasReferenceId;

        package.TransferredAt = now;
        package.AtlasReferenceId = atlasReferenceId;
        package.NextAttemptAt = null;
        package.LastError = null;

        document.SubmissionStatus = (int)PublishStatusEnum.Basarili;
        document.SubmissionUpdatedAt = now;
        document.Release = true;
        document.ReleaseDate = now;

        await AddResultTransactionAsync(document.Id, TransactionTypeEnum.AtlasAktarildi, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return TransferOutcome.Succeeded;
    }

    private async Task<TransferOutcome> FinishWithFailureAsync(
        IncomingDocument document,
        EypPackage package,
        EypTransfer attempt,
        string error,
        bool permanent,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        error = error.Length > ErrorMaxLength ? error[..ErrorMaxLength] : error;

        attempt.Error = error;
        attempt.IsPermanentFailure = permanent;
        package.LastError = error;
        document.SubmissionUpdatedAt = now;

        if (permanent)
        {
            package.NextAttemptAt = null;
            document.SubmissionStatus = (int)PublishStatusEnum.Hatali;
            await AddResultTransactionAsync(document.Id, TransactionTypeEnum.AtlasAktarimHatali, cancellationToken);
        }
        else
        {
            package.NextAttemptAt = now + RetryDelays[Math.Min(package.TryCount, RetryDelays.Length) - 1];
            document.SubmissionStatus = (int)PublishStatusEnum.Kuyrukta;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return permanent ? TransferOutcome.Failed : TransferOutcome.Requeued;
    }

    // Kullanıcı olarak evrakı en son yayınlayan kişi yazılır
    private async Task AddResultTransactionAsync(
        Guid documentId,
        TransactionTypeEnum transactionType,
        CancellationToken cancellationToken)
    {
        var publisherId = await _context.Set<DocumentTransaction>()
            .Where(x => x.DocumentId == documentId && x.TransactionType == (int)TransactionTypeEnum.Yayinla)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        _context.Add(new DocumentTransaction
        {
            DocumentId = documentId,
            TransactionType = (int)transactionType,
            UserId = publisherId,
            IsActive = true
        });
    }

    // Kuyruktaki evraklardan en eskileri atomik olarak Aktarılıyor yapılır. READPAST sayesinde
    // aynı anda çalışan iki tur (ör. iki uygulama örneği) aynı evrakı almaz. Geçici hatadan sonra
    // bekleme süresi dolmamış evraklar atlanır.
    private async Task<List<Guid>> ClaimAsync(int count, CancellationToken cancellationToken)
    {
        if (count <= 0)
            return new List<Guid>();

        var now = DateTime.UtcNow;

        return await _context.Database.SqlQuery<Guid>($"""
            WITH NextDocuments AS (
                SELECT TOP ({count}) d.*
                FROM IncomingDocuments d WITH (UPDLOCK, READPAST, ROWLOCK)
                WHERE d.SubmissionStatus = {(int)PublishStatusEnum.Kuyrukta} AND d.IsDeleted = 0
                  AND NOT EXISTS (
                      SELECT 1 FROM EypPackages p
                      WHERE p.DocumentId = d.Id AND p.IsDeleted = 0 AND p.NextAttemptAt > {now})
                ORDER BY d.SubmissionUpdatedAt, d.CreatedDate
            )
            UPDATE NextDocuments
            SET SubmissionStatus = {(int)PublishStatusEnum.Aktariliyor},
                SubmissionUpdatedAt = {now},
                UpdateDate = {now}
            OUTPUT inserted.Id AS Value
            """).ToListAsync(cancellationToken);
    }

    private async Task<int> RecoverStuckAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var threshold = now - StuckAfter;

        var recovered = await _context.Set<IncomingDocument>()
            .Where(x => x.SubmissionStatus == (int)PublishStatusEnum.Aktariliyor
                        && !x.IsDeleted
                        && (x.SubmissionUpdatedAt == null || x.SubmissionUpdatedAt < threshold))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.SubmissionStatus, (int)PublishStatusEnum.Kuyrukta)
                .SetProperty(x => x.SubmissionUpdatedAt, now)
                .SetProperty(x => x.UpdateDate, now),
                cancellationToken);

        if (recovered > 0)
            _logger.LogWarning("{Count} evrak uzun süre Aktarılıyor durumunda kaldığı için kuyruğa geri alındı", recovered);

        return recovered;
    }

    private async Task<EypRecipient?> GetRecipientAsync(CancellationToken cancellationToken)
    {
        var kkk = await _appSettingService.GetValueAsync(AppSettingKeys.AtlasEypRecipientKkk, cancellationToken);
        var name = await _appSettingService.GetValueAsync(AppSettingKeys.AtlasEypRecipientName, cancellationToken);

        return string.IsNullOrWhiteSpace(kkk) || string.IsNullOrWhiteSpace(name)
            ? null
            : new EypRecipient(kkk.Trim(), name.Trim());
    }

    private static AtlasTransferRunResultDto NotRun(string message) => new(false, 0, 0, 0, 0, 0, message);

    private async Task<int> GetIntSettingAsync(string key, int defaultValue, CancellationToken cancellationToken)
    {
        var value = await _appSettingService.GetValueAsync(key, cancellationToken);
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }

    private async Task<bool> GetBoolSettingAsync(string key, bool defaultValue, CancellationToken cancellationToken)
    {
        var value = await _appSettingService.GetValueAsync(key, cancellationToken);
        return bool.TryParse(value, out var parsed) ? parsed : defaultValue;
    }

    private enum TransferOutcome
    {
        Succeeded,
        Requeued,
        Failed,
        Skipped
    }
}
