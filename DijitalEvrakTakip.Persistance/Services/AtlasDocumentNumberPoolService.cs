using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class AtlasDocumentNumberPoolService : IAtlasDocumentNumberPoolService
{
    // Arka plan servisi, elle doldurma ve ayırma sırasındaki doldurma aynı anda Atlas'a gitmesin
    private static readonly SemaphoreSlim Gate = new(1, 1);

    // Numara ayırırken havuz boşsa, süren bir doldurma turunun bitmesi en fazla bu kadar beklenir
    private static readonly TimeSpan ReserveRefillWait = TimeSpan.FromSeconds(30);

    private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(300);
    private static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(30);

    private const int DefaultMinStock = 100;
    private const int DefaultBatchSize = 100;
    private const int MaxBatchSize = 1000;
    private const int QrCodeMaxLength = 50; // AtlasDocumentNumbers.QrCode kolon uzunluğu

    private readonly IAtlasEbysClient _atlasClient;
    private readonly IAppSettingService _appSettingService;
    private readonly AppDbContext _context;
    private readonly ILogger<AtlasDocumentNumberPoolService> _logger;

    public AtlasDocumentNumberPoolService(
        IAtlasEbysClient atlasClient,
        IAppSettingService appSettingService,
        AppDbContext context,
        ILogger<AtlasDocumentNumberPoolService> logger)
    {
        _atlasClient = atlasClient;
        _appSettingService = appSettingService;
        _context = context;
        _logger = logger;
    }

    public async Task<TimeSpan> GetIntervalAsync(CancellationToken cancellationToken)
    {
        var seconds = await GetIntSettingAsync(
            AppSettingKeys.AtlasNumberPoolIntervalSeconds, (int)DefaultInterval.TotalSeconds, cancellationToken);

        var interval = TimeSpan.FromSeconds(seconds);
        return interval < MinInterval ? MinInterval : interval;
    }

    public Task<AtlasDocumentNumberRefillResultDto> RefillAsync(
        bool ignoreEnabledFlag,
        CancellationToken cancellationToken)
    {
        return RefillCoreAsync(ignoreEnabledFlag, ignoreMinStock: false, TimeSpan.Zero, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> ReserveAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken)
    {
        var numbers = await ReserveFromPoolAsync(userId, count, cancellationToken);

        if (numbers.Count < count)
        {
            // Havuz yetmedi: ayar kapalı ve stok minimumun üstünde olsa da Atlas'tan bir paket istenir
            await RefillCoreAsync(ignoreEnabledFlag: true, ignoreMinStock: true, ReserveRefillWait, cancellationToken);
            numbers.AddRange(await ReserveFromPoolAsync(userId, count - numbers.Count, cancellationToken));
        }

        return numbers;
    }

    public async Task<AtlasDocumentNumberPoolStockDto> GetStockAsync(CancellationToken cancellationToken)
    {
        var counts = await _context.Set<AtlasDocumentNumber>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

        int CountOf(AtlasDocumentNumberStatusEnum status) => counts.GetValueOrDefault((int)status);

        var minStock = await GetIntSettingAsync(AppSettingKeys.AtlasNumberPoolMinStock, DefaultMinStock, cancellationToken);

        return new AtlasDocumentNumberPoolStockDto(
            Available: CountOf(AtlasDocumentNumberStatusEnum.Available),
            Reserved: CountOf(AtlasDocumentNumberStatusEnum.Reserved),
            Used: CountOf(AtlasDocumentNumberStatusEnum.Used),
            Cancelled: CountOf(AtlasDocumentNumberStatusEnum.Cancelled),
            MinStock: minStock);
    }

    public async Task CancelAsync(
        string qrCode,
        Guid? userId,
        string? reason,
        CancellationToken cancellationToken)
    {
        var entity = await _context.Set<AtlasDocumentNumber>()
            .FirstOrDefaultAsync(x => x.QrCode == qrCode && !x.IsDeleted, cancellationToken)
            ?? throw new Exception($"{qrCode} numarası havuzda bulunamadı.");

        if (entity.Status == (int)AtlasDocumentNumberStatusEnum.Used)
            throw new Exception($"{qrCode} numarası bir evrakta kullanılmış, iptal edilemez.");

        if (entity.Status == (int)AtlasDocumentNumberStatusEnum.Cancelled)
            throw new Exception($"{qrCode} numarası zaten iptal edilmiş.");

        entity.Status = (int)AtlasDocumentNumberStatusEnum.Cancelled;
        entity.CancelledByUserId = userId;
        entity.CancelledAt = DateTime.UtcNow;
        entity.CancelReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkUsedForIncomingDocumentAsync(
        string qrCode,
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        qrCode = qrCode.Trim();

        var entity = await _context.Set<AtlasDocumentNumber>()
            .FirstOrDefaultAsync(x => x.QrCode == qrCode && !x.IsDeleted, cancellationToken);

        if (entity is null)
        {
            // Geçiş döneminde havuz dışındaki (elle girilen / eski etiketli) numaralar kabul edilir
            if (await GetBoolSettingAsync(AppSettingKeys.AtlasNumberPoolEnforced, false, cancellationToken))
                throw new Exception($"{qrCode} numarası Atlas'tan alınmış bir evrak numarası değil.");

            return;
        }

        switch ((AtlasDocumentNumberStatusEnum)entity.Status)
        {
            case AtlasDocumentNumberStatusEnum.Used when entity.IncomingDocumentId == incomingDocumentId:
                return;
            case AtlasDocumentNumberStatusEnum.Used:
                throw new Exception($"{qrCode} numarası başka bir evrakta kullanılmış.");
            case AtlasDocumentNumberStatusEnum.Cancelled:
                throw new Exception($"{qrCode} numarası iptal edilmiş, kullanılamaz.");
        }

        entity.Status = (int)AtlasDocumentNumberStatusEnum.Used;
        entity.IncomingDocumentId = incomingDocumentId;
        entity.UsedAt = DateTime.UtcNow;
    }

    private async Task<AtlasDocumentNumberRefillResultDto> RefillCoreAsync(
        bool ignoreEnabledFlag,
        bool ignoreMinStock,
        TimeSpan gateWait,
        CancellationToken cancellationToken)
    {
        if (!await Gate.WaitAsync(gateWait, cancellationToken))
            return new(false, 0, 0, 0, await CountAvailableAsync(cancellationToken), "Havuz doldurma şu anda zaten çalışıyor.");

        try
        {
            if (!ignoreEnabledFlag
                && !await GetBoolSettingAsync(AppSettingKeys.AtlasNumberPoolEnabled, false, cancellationToken))
                return new(false, 0, 0, 0, 0, "Otomatik havuz doldurma kapalı.");

            var available = await CountAvailableAsync(cancellationToken);
            var minStock = await GetIntSettingAsync(AppSettingKeys.AtlasNumberPoolMinStock, DefaultMinStock, cancellationToken);

            if (!ignoreMinStock && available >= minStock)
                return new(false, 0, 0, 0, available, $"Stok yeterli ({available} / en az {minStock}).");

            var batchSize = Math.Min(
                await GetIntSettingAsync(AppSettingKeys.AtlasNumberPoolBatchSize, DefaultBatchSize, cancellationToken),
                MaxBatchSize);

            IReadOnlyList<AtlasIssuedNumberDto> issued;
            try
            {
                issued = await _atlasClient.RequestNumbersAsync(batchSize, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Atlas EBYS'den evrak numarası alınamadı");
                return new(true, batchSize, 0, 0, available, $"Atlas'tan numara alınamadı: {ex.Message}");
            }

            var candidates = issued
                .Where(x => !string.IsNullOrWhiteSpace(x.QrCode))
                .Select(x => x with { QrCode = x.QrCode.Trim() })
                .Where(x => x.QrCode.Length <= QrCodeMaxLength)
                .DistinctBy(x => x.QrCode)
                .ToList();

            var candidateCodes = candidates.Select(x => x.QrCode).ToList();
            var existingCodes = await _context.Set<AtlasDocumentNumber>()
                .Where(x => candidateCodes.Contains(x.QrCode))
                .Select(x => x.QrCode)
                .ToListAsync(cancellationToken);

            var newNumbers = candidates
                .Where(x => !existingCodes.Contains(x.QrCode))
                .Select(x => new AtlasDocumentNumber
                {
                    QrCode = x.QrCode,
                    AtlasReferenceId = x.AtlasReferenceId,
                    Status = (int)AtlasDocumentNumberStatusEnum.Available
                })
                .ToList();

            await _context.Set<AtlasDocumentNumber>().AddRangeAsync(newNumbers, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var duplicates = issued.Count - newNumbers.Count;
            if (duplicates > 0)
                _logger.LogWarning("Atlas'tan gelen {Count} numara boş, çok uzun veya havuzda zaten var; eklenmedi", duplicates);

            var availableAfter = available + newNumbers.Count;
            return new(true, batchSize, newNumbers.Count, duplicates, availableAfter,
                $"{newNumbers.Count} numara havuza eklendi, boştaki numara sayısı {availableAfter}.");
        }
        finally
        {
            Gate.Release();
        }
    }

    // Boştaki numaralardan en eskileri atomik olarak ayrılır. READPAST sayesinde aynı anda
    // çalışan iki istek birbirinin kilitlediği satırları atlar; aynı numara iki kullanıcıya verilmez.
    private async Task<List<string>> ReserveFromPoolAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken)
    {
        if (count <= 0)
            return new List<string>();

        var now = DateTime.UtcNow;

        var numbers = await _context.Database.SqlQuery<string>($"""
            WITH NextNumbers AS (
                SELECT TOP ({count}) *
                FROM AtlasDocumentNumbers WITH (UPDLOCK, READPAST, ROWLOCK)
                WHERE Status = {(int)AtlasDocumentNumberStatusEnum.Available} AND IsDeleted = 0
                ORDER BY CreatedDate, QrCode
            )
            UPDATE NextNumbers
            SET Status = {(int)AtlasDocumentNumberStatusEnum.Reserved},
                ReservedByUserId = {userId},
                ReservedAt = {now},
                UpdateDate = {now}
            OUTPUT inserted.QrCode AS Value
            """).ToListAsync(cancellationToken);

        numbers.Sort(StringComparer.Ordinal);
        return numbers;
    }

    private Task<int> CountAvailableAsync(CancellationToken cancellationToken) =>
        _context.Set<AtlasDocumentNumber>()
            .CountAsync(x => x.Status == (int)AtlasDocumentNumberStatusEnum.Available && !x.IsDeleted, cancellationToken);

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
}
