using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.WebApi.BackgroundServices;

/// <summary>
/// Yayınlanan gelen evrakları belirli aralıklarla kuyruktan alıp EYP paketi üretir ve Atlas'a aktarır.
/// Ayarlar her turda yeniden okunduğu için açma/kapama ve aralık değişikliği yeniden başlatma gerektirmez.
/// </summary>
public sealed class AtlasTransferBackgroundService : BackgroundService
{
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan FallbackInterval = TimeSpan.FromMinutes(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AtlasTransferBackgroundService> _logger;

    public AtlasTransferBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<AtlasTransferBackgroundService> logger)
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
                var transferService = scope.ServiceProvider.GetRequiredService<IAtlasTransferService>();

                var result = await transferService.RunAsync(ignoreEnabledFlag: false, stoppingToken);
                if (result.Ran && (result.Picked > 0 || result.RecoveredStuck > 0))
                    _logger.LogInformation("Atlas aktarımı: {Message}", result.Message);

                interval = await transferService.GetIntervalAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atlas aktarım turu başarısız oldu");
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
