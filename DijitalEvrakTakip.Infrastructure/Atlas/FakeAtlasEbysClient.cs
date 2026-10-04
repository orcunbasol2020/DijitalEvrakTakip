using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Infrastructure.Atlas;

/// <summary>
/// Atlas EBYS web servisi sözleşmesi netleşene kadar kullanılan sahte istemci.
/// Atlas'ın biçiminde ("yıl/8 hane", ör. 2026/42679909) numara üretir. Gerçek numaralarla
/// karışmasın diye sıra numarası 9 ile başlar (2026/9xxxxxxx).
/// </summary>
public sealed class FakeAtlasEbysClient : IAtlasEbysClient
{
    private const int FakeRangeStart = 90_000_000;
    private const int FakeRangeSize = 10_000_000;

    // Açılış anındaki saniyeden başlayıp artan sayaç; yeniden başlatmada çakışan numaralar
    // havuza eklenirken ayıklanır
    private static long _counter = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public Task<IReadOnlyList<AtlasIssuedNumberDto>> RequestNumbersAsync(int count, CancellationToken cancellationToken)
    {
        var year = DateTime.Now.Year;
        var numbers = new List<AtlasIssuedNumberDto>(count);

        for (var i = 0; i < count; i++)
        {
            var sequence = FakeRangeStart + Interlocked.Increment(ref _counter) % FakeRangeSize;
            numbers.Add(new AtlasIssuedNumberDto($"{year}/{sequence:D8}", AtlasReferenceId: null));
        }

        return Task.FromResult<IReadOnlyList<AtlasIssuedNumberDto>>(numbers);
    }
}
