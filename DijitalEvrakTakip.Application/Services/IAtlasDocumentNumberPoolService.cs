using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

public interface IAtlasDocumentNumberPoolService
{
    /// <summary>
    /// Boştaki numara sayısı minimum stoğun altındaysa Atlas'tan bir paket numara çeker.
    /// <paramref name="ignoreEnabledFlag"/> true ise havuz ayarı kapalı olsa da çalışır.
    /// </summary>
    Task<AtlasDocumentNumberRefillResultDto> RefillAsync(bool ignoreEnabledFlag, CancellationToken cancellationToken);

    /// <summary>Arka plan servisinin iki tur arasında bekleyeceği süre.</summary>
    Task<TimeSpan> GetIntervalAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Etiket basmak için boştaki numaralardan <paramref name="count"/> adedini kullanıcıya ayırır.
    /// Aynı numara iki kullanıcıya verilmez. Havuz yetmezse bir kez Atlas'tan doldurmayı dener.
    /// </summary>
    Task<IReadOnlyList<string>> ReserveAsync(Guid userId, int count, CancellationToken cancellationToken);

    Task<AtlasDocumentNumberPoolStockDto> GetStockAsync(CancellationToken cancellationToken);

    /// <summary>Hatalı basılan / kullanılmayacak numarayı iptal eder. Kullanılmış numara iptal edilemez.</summary>
    Task CancelAsync(string qrCode, Guid? userId, string? reason, CancellationToken cancellationToken);

    /// <summary>
    /// Numarayla gelen evrak açılırken çağrılır: havuzdaki numarayı Kullanıldı yapar.
    /// SaveChanges çağırmaz; evrak kaydıyla aynı SaveChanges içinde yazılır.
    /// AtlasNumberPoolEnforced açıkken havuzda olmayan numara reddedilir; kullanılmış veya
    /// iptal edilmiş numara her durumda reddedilir.
    /// </summary>
    Task MarkUsedForIncomingDocumentAsync(string qrCode, Guid incomingDocumentId, CancellationToken cancellationToken);
}
