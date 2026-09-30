using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.WebApi.BackgroundServices;

/// <summary>
/// Onay bekleyen zimmet taleplerini belirli aralıklarla kontrol eder; hatırlatma zamanı
/// gelenler için alıcıya, eşiği aşanlar için devredene uygulama içi bildirim üretir.
/// Açma/kapama, hatırlatma aralığı ve mesai saatleri AppSettings'ten her turda okunur.
/// </summary>
public sealed class ZimmetApprovalReminderBackgroundService : BackgroundService
{
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ZimmetApprovalReminderBackgroundService> _logger;

    public ZimmetApprovalReminderBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ZimmetApprovalReminderBackgroundService> logger)
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
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var requestService = scope.ServiceProvider.GetRequiredService<IDocumentAllocationRequestService>();

                var reminded = await requestService.SendRemindersAsync(stoppingToken);
                if (reminded > 0)
                    _logger.LogInformation("{Count} zimmet talebi için hatırlatma bildirimi üretildi", reminded);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Zimmet onay hatırlatma turu başarısız oldu");
            }

            try
            {
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
