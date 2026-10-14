namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Dosyası yüklenmemiş (genelde gizlilik dereceli, yalnızca bilgileri girilmiş) evrakların EYP üst yazısı:
/// "Bu Belge Fiziki Olarak Gönderilecektir." PDF'i. Dosya şablon klasöründen (FileStorage:PhysicalDeliveryPdfPath) okunur.
/// </summary>
public interface IPhysicalDeliveryPdfProvider
{
    /// <summary>PDF'in yolu (durum ekranında gösterilir).</summary>
    string Path { get; }

    /// <summary>PDF yolda duruyor mu.</summary>
    bool IsAvailable { get; }

    Task<byte[]> GetAsync(CancellationToken cancellationToken);
}
