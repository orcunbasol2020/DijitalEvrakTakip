using DijitalEvrakTakip.Application.Services;

namespace DijitalEvrakTakip.Persistance.Services;

/// <summary>
/// Fiziki gönderim PDF'ini şablon klasöründen okur. Dosya her pakette okunur; şablon değiştirilince
/// uygulamayı yeniden başlatmak gerekmez. Yeniden üretmek için Resources/fiziki_gonderim_pdf.ps1.
/// </summary>
public sealed class PhysicalDeliveryPdfProvider : IPhysicalDeliveryPdfProvider
{
    public PhysicalDeliveryPdfProvider(string path)
    {
        Path = path;
    }

    public string Path { get; }

    public bool IsAvailable => File.Exists(Path);

    public Task<byte[]> GetAsync(CancellationToken cancellationToken)
    {
        if (!IsAvailable)
            throw new InvalidOperationException($"Fiziki gönderim PDF'i bulunamadı: {Path}");

        return File.ReadAllBytesAsync(Path, cancellationToken);
    }
}
