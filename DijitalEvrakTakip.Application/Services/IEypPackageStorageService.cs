namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Üretilen EYP paketlerinin tutulduğu klasöre erişim sağlar. Paketler aktarımdan sonra da
/// arşiv olarak saklanır.
/// </summary>
public interface IEypPackageStorageService
{
    /// <summary>Paketi kaydeder ve kaydedilen dosya adını (klasörsüz) döner.</summary>
    Task<string> SaveAsync(string qrCode, Guid packageId, byte[] content, CancellationToken cancellationToken);

    Task<byte[]> ReadAsync(string fileName, CancellationToken cancellationToken);

    bool Exists(string fileName);
}
