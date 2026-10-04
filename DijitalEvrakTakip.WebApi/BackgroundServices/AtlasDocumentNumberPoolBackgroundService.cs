using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.WebApi.BackgroundServices;

/// <summary>
/// Atlas evrak numarası havuzunu belirli aralıklarla kontrol eder; boştaki numara sayısı
/// minimum stoğun altına düşünce Atlas'tan yeni paket çeker. Ayarlar her turda yeniden
/// okunduğu için açma/kapama ve aralık değişikliği yeniden başlatma gerektirmez.
/// </summary>
public sealed class AtlasDocumentNumberPoolBackgroundService : BackgroundService
{
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan FallbackInterval = TimeSpan.FromMinutes(5);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AtlasDocumentNumberPoolBackgroundService> _logger;

    public AtlasDocumentNumberPoolBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<AtlasDocumentNumberPoolBackgroundService> logger)
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
                var poolService = scope.ServiceProvider.GetRequiredService<IAtlasDocumentNumberPoolService>();

                var result = await poolService.RefillAsync(ignoreEnabledFlag: false, stoppingToken);
                if (result.Ran)
                    _logger.LogInformation("Atlas numara havuzu: {Message}", result.Message);

                interval = await poolService.GetIntervalAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atlas numara havuzu doldurma turu başarısız oldu");
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
