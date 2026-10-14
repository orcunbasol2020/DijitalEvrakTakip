using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Atlas aktarım turlarını uygulama belleğinde izler: çalışan tur, tamamlanan son tur ve arka plan
/// servisinin sonraki turu. Veritabanına yazılmaz; uygulama yeniden başlayınca boşalır ve birden fazla
/// uygulama örneği varsa her biri yalnızca kendi turlarını bilir.
/// </summary>
public interface IAtlasTransferMonitor
{
    void RunStarted(bool manual);

    void RunCompleted(AtlasTransferRunResultDto result);

    /// <summary>Tur beklenmeyen bir hatayla kesildi.</summary>
    void RunFailed(string error);

    void NextRunScheduled(DateTime nextRunAt);

    AtlasTransferMonitorSnapshot GetSnapshot();
}

public sealed record AtlasTransferMonitorSnapshot(
    bool IsRunning,
    DateTime? RunningSince,
    AtlasTransferLastRunDto? LastRun,
    DateTime? NextRunAt);
