using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Infrastructure.Atlas;

/// <summary>Bellekteki tur izleyicisi; singleton olarak kaydedilir.</summary>
public sealed class AtlasTransferMonitor : IAtlasTransferMonitor
{
    private readonly object _lock = new();

    private DateTime? _runningSince;
    private bool _runningManual;
    private AtlasTransferLastRunDto? _lastRun;
    private DateTime? _nextRunAt;

    public void RunStarted(bool manual)
    {
        lock (_lock)
        {
            _runningSince = DateTime.UtcNow;
            _runningManual = manual;
        }
    }

    public void RunCompleted(AtlasTransferRunResultDto result) => Finish(result, null);

    public void RunFailed(string error) => Finish(null, error);

    public void NextRunScheduled(DateTime nextRunAt)
    {
        lock (_lock)
            _nextRunAt = nextRunAt;
    }

    public AtlasTransferMonitorSnapshot GetSnapshot()
    {
        lock (_lock)
            return new AtlasTransferMonitorSnapshot(_runningSince is not null, _runningSince, _lastRun, _nextRunAt);
    }

    private void Finish(AtlasTransferRunResultDto? result, string? error)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            _lastRun = new AtlasTransferLastRunDto(_runningSince ?? now, now, _runningManual, result, error);
            _runningSince = null;
        }
    }
}
