using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.WebApi.BackgroundServices;

/// <summary>
/// AppSettings'teki tarama klasörünü belirli aralıklarla kontrol eder ve
/// bulunan PDF'leri taranmış belge olarak içe aktarır. Ayarlar her turda
/// yeniden okunduğu için açma/kapama ve aralık değişikliği yeniden başlatma gerektirmez.
/// </summary>
public sealed class ScannedDocumentImportBackgroundService : BackgroundService
{
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan FallbackInterval = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScannedDocumentImportBackgroundService> _logger;

    public ScannedDocumentImportBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ScannedDocumentImportBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(StartupDelay, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var interval = FallbackInterval;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var importService = scope.ServiceProvider.GetRequiredService<IScannedDocumentImportService>();

                await importService.ImportAsync(ignoreEnabledFlag: false, stoppingToken);
                interval = await importService.GetIntervalAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Taranan belge içe aktarma turu başarısız oldu");
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
