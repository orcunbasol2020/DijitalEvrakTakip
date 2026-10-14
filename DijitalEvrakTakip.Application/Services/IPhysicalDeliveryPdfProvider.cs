namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Dosyası yüklenmemiş (genelde gizlilik dereceli, yalnızca bilgileri girilmiş) evrakların EYP üst yazısı:
/// "Bu Belge Fiziki Olarak Gönderilecektir." PDF'i. Dosya şablon klasöründen (FileStorage:PhysicalDeliveryPdfPath) okunur.
/// </summary>
public interface IPhysicalDeliveryPdfProvider
{
    Task<byte[]> GetAsync(CancellationToken cancellationToken);
}
