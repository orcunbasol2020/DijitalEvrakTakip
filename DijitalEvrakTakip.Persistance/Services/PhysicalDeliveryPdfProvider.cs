using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.Persistance.Services;

/// <summary>
/// Fiziki gönderim PDF'ini şablon klasöründen okur. Dosya her pakette okunur; şablon değiştirilince
/// uygulamayı yeniden başlatmak gerekmez. Yeniden üretmek için Resources/fiziki_gonderim_pdf.ps1.
/// </summary>
public sealed class PhysicalDeliveryPdfProvider : IPhysicalDeliveryPdfProvider
{
    private readonly string _path;

    public PhysicalDeliveryPdfProvider(string path)
    {
        _path = path;
    }

    public Task<byte[]> GetAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_path))
            throw new InvalidOperationException($"Fiziki gönderim PDF'i bulunamadı: {_path}");

        return File.ReadAllBytesAsync(_path, cancellationToken);
    }
}
