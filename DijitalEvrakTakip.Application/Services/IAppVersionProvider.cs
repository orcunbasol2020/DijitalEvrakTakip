namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Deploy edilen koda ait versiyon bilgisi. Veritabanında değil assembly'de tutulur,
/// böylece her zaman çalışan kodla tutarlıdır.
/// </summary>
public interface IAppVersionProvider
{
    string Version { get; }

    DateTime? BuildDate { get; }
}
