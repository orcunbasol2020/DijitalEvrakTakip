using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Persistance.Context;
using Microsoft.Extensions.Logging;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class ScannedDocumentImportService : IScannedDocumentImportService
{
    // Arka plan servisi ile elle tetikleme aynı anda aynı dosyaları işlemesin
    private static readonly SemaphoreSlim Gate = new(1, 1);

    private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(10);

    // Tarayıcı hâlâ yazıyor olabilir: son değişikliği bu süreden yeni olan dosyalar bir sonraki tura kalır
    private static readonly TimeSpan StableAge = TimeSpan.FromSeconds(10);

    private const string ErrorFolderName = "Error";
    private const int PathMaxLength = 200; // ScannedDocuments.NewPath / OriginalPath / FileName kolon uzunluğu

    private readonly IAppSettingService _appSettingService;
    private readonly IIncomingDocumentStorageService _storageService;
    private readonly AppDbContext _context;
    private readonly ILogger<ScannedDocumentImportService> _logger;

    public ScannedDocumentImportService(
        IAppSettingService appSettingService,
        IIncomingDocumentStorageService storageService,
        AppDbContext context,
        ILogger<ScannedDocumentImportService> logger)
    {
        _appSettingService = appSettingService;
        _storageService = storageService;
        _context = context;
        _logger = logger;
    }

    public async Task<TimeSpan> GetIntervalAsync(CancellationToken cancellationToken)
    {
        var value = await _appSettingService.GetValueAsync(AppSettingKeys.ScanImportIntervalSeconds, cancellationToken);

        if (!int.TryParse(value, out var seconds) || seconds <= 0)
            return DefaultInterval;

        var interval = TimeSpan.FromSeconds(seconds);
        return interval < MinInterval ? MinInterval : interval;
    }

    public async Task<ScannedDocumentImportResultDto> ImportAsync(
        bool ignoreEnabledFlag,
        CancellationToken cancellationToken)
    {
        if (!await Gate.WaitAsync(0, cancellationToken))
            return new(false, 0, 0, 0, "İçe aktarma şu anda zaten çalışıyor.");

        try
        {
            if (!ignoreEnabledFlag)
            {
                var enabledValue = await _appSettingService.GetValueAsync(AppSettingKeys.ScanImportEnabled, cancellationToken);
                if (!bool.TryParse(enabledValue, out var enabled) || !enabled)
                    return new(false, 0, 0, 0, "Otomatik içe aktarma kapalı.");
            }

            var folder = await _appSettingService.GetValueAsync(AppSettingKeys.ScanImportFolderPath, cancellationToken);
            if (string.IsNullOrWhiteSpace(folder))
                return new(false, 0, 0, 0, "Tarama klasörü ayarı boş.");

            folder = folder.Trim();
            if (!Directory.Exists(folder))
            {
                _logger.LogWarning("Tarama klasörü bulunamadı: {Folder}", folder);
                return new(false, 0, 0, 0, $"Tarama klasörü bulunamadı: {folder}");
            }

            // Tarama klasörü depolama klasörüyle aynıysa taşınan her dosya yeniden bulunur ve
            // tur hiç bitmeden yeni kayıt üretir; bu durumda içe aktarma yapılmaz
            var storageRoot = Path.GetDirectoryName(_storageService.GetFullPath("x"))!;
            if (string.Equals(
                    Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder)),
                    Path.TrimEndingDirectorySeparator(Path.GetFullPath(storageRoot)),
                    StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Tarama klasörü belge depolama klasörüyle aynı: {Folder}", folder);
                return new(false, 0, 0, 0, "Tarama klasörü, belgelerin taşındığı depolama klasörüyle aynı olamaz.");
            }

            int imported = 0, skipped = 0, failed = 0;

            // Yalnızca üst klasör: Error alt klasörü tekrar işlenmez.
            // Liste tur başında alınır; tur sırasında klasöre düşen dosyalar bir sonraki tura kalır.
            foreach (var path in Directory.GetFiles(folder, "*.pdf", SearchOption.TopDirectoryOnly))
            {
                cancellationToken.ThrowIfCancellationRequested();

                switch (await ImportFileAsync(folder, path, cancellationToken))
                {
                    case FileResult.Imported: imported++; break;
                    case FileResult.Skipped: skipped++; break;
                    default: failed++; break;
                }
            }

            if (imported > 0 || failed > 0)
                _logger.LogInformation(
                    "Taranan belge içe aktarma: {Imported} alındı, {Skipped} atlandı, {Failed} hatalı",
                    imported, skipped, failed);

            return new(true, imported, skipped, failed,
                $"{imported} belge içe aktarıldı, {skipped} belge sonraki tura bırakıldı, {failed} belge hatalı.");
        }
        finally
        {
            Gate.Release();
        }
    }

    private async Task<FileResult> ImportFileAsync(string folder, string sourcePath, CancellationToken cancellationToken)
    {
        var info = new FileInfo(sourcePath);

        if (DateTime.UtcNow - info.LastWriteTimeUtc < StableAge || !CanOpenExclusively(sourcePath))
            return FileResult.Skipped;

        var originalName = info.Name;
        var targetName = BuildTargetFileName(originalName);
        var targetPath = _storageService.GetFullPath(targetName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
            File.Move(sourcePath, targetPath);
        }
        catch (IOException)
        {
            // Dosya bu arada başka bir süreç tarafından alınmış veya kilitlenmiş olabilir
            return FileResult.Skipped;
        }
        catch (Exception ex)
        {
            await LogErrorAsync($"Taranan belge taşınamadı: {originalName}", ex);
            return FileResult.Failed;
        }

        try
        {
            _context.Set<ScannedDocument>().Add(new ScannedDocument
            {
                FileName = targetName,
                OriginalPath = Truncate(originalName),
                NewPath = targetPath
            });

            await _context.SaveChangesAsync(cancellationToken);

            // Kaydedilen varlıklar izlenmeye devam etmesin; kalabalık bir turda bellek ve
            // DetectChanges maliyeti dosya sayısıyla birlikte büyümesin
            _context.ChangeTracker.Clear();
            return FileResult.Imported;
        }
        catch (Exception ex)
        {
            // Kayıt oluşmadıysa dosyayı Error klasörüne al; her turda tekrar denenmesin
            _context.ChangeTracker.Clear();
            MoveToErrorFolder(folder, targetPath, originalName);
            await LogErrorAsync($"Taranan belge kaydedilemedi: {originalName}", ex);
            return FileResult.Failed;
        }
    }

    // Kayıtlı ad: <orijinal ad>_<guid>.pdf; NewPath kolon sınırına sığacak şekilde kısaltılır
    private string BuildTargetFileName(string originalName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var stem = string.Concat(Path.GetFileNameWithoutExtension(originalName).Where(c => !invalid.Contains(c)));

        var suffix = $"_{Guid.NewGuid():N}.pdf";
        var rootLength = _storageService.GetFullPath("x").Length - 1;
        var maxStemLength = Math.Max(0, PathMaxLength - rootLength - suffix.Length);

        if (stem.Length > maxStemLength)
            stem = stem[..maxStemLength];

        return $"{stem}{suffix}".TrimStart('_');
    }

    private void MoveToErrorFolder(string folder, string currentPath, string originalName)
    {
        try
        {
            var errorFolder = Path.Combine(folder, ErrorFolderName);
            Directory.CreateDirectory(errorFolder);

            var errorPath = Path.Combine(errorFolder, originalName);
            if (File.Exists(errorPath))
                errorPath = Path.Combine(errorFolder,
                    $"{Path.GetFileNameWithoutExtension(originalName)}_{Guid.NewGuid():N}{Path.GetExtension(originalName)}");

            File.Move(currentPath, errorPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hatalı taranan belge Error klasörüne taşınamadı: {Path}", currentPath);
        }
    }

    private async Task LogErrorAsync(string message, Exception ex)
    {
        _logger.LogError(ex, "{Message}", message);

        try
        {
            _context.ChangeTracker.Clear();
            _context.Set<ErrorLog>().Add(new ErrorLog
            {
                ErrorMessage = $"{message} - {ex.Message}",
                StackTrace = ex.StackTrace ?? string.Empty,
                RequestPath = "ScannedDocumentImport",
                RequestMethod = "BACKGROUND",
                TimeStamp = DateTime.UtcNow,
                UserId = "system"
            });
            await _context.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception logEx)
        {
            _context.ChangeTracker.Clear();
            _logger.LogError(logEx, "Hata kaydı veritabanına yazılamadı");
        }
    }

    private static bool CanOpenExclusively(string path)
    {
        try
        {
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static string Truncate(string value)
        => value.Length <= PathMaxLength ? value : value[..PathMaxLength];

    private enum FileResult
    {
        Imported,
        Skipped,
        Failed
    }
}
