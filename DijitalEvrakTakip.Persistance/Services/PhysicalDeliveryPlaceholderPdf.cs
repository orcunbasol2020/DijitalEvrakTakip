namespace DijitalEvrakTakip.Persistance.Services;

/// <summary>
/// Dosyası yüklenmemiş (yalnızca bilgileri girilmiş) evrakların EYP üst yazısı olarak kullanılan
/// sabit PDF: Bakanlık logosu ve "Bu Belge Fiziki Olarak Gönderilecektir." ibaresi.
/// Kaynak dosya Resources/fiziki_gonderim.pdf, üretim betiği Resources/fiziki_gonderim_pdf.ps1.
/// </summary>
internal static class PhysicalDeliveryPlaceholderPdf
{
    private const string ResourceName = "DijitalEvrakTakip.Persistance.Resources.fiziki_gonderim.pdf";

    private static readonly Lazy<byte[]> Content = new(Load);

    public static byte[] Get() => Content.Value;

    private static byte[] Load()
    {
        using var stream = typeof(PhysicalDeliveryPlaceholderPdf).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Gömülü kaynak bulunamadı: {ResourceName}");

        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }
}
