using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Atlas EBYS web servisiyle konuşan istemci. Veritabanına dokunmaz; havuz yönetimi
/// IAtlasDocumentNumberPoolService'tedir. Gerçek servis sözleşmesi netleşene kadar
/// FakeAtlasEbysClient kullanılır.
/// </summary>
public interface IAtlasEbysClient
{
    /// <summary>Atlas'tan <paramref name="count"/> adet yeni evrak numarası ister.</summary>
    Task<IReadOnlyList<AtlasIssuedNumberDto>> RequestNumbersAsync(int count, CancellationToken cancellationToken);
}
