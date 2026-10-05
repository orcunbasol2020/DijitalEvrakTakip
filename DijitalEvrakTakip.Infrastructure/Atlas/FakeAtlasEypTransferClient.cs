using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Infrastructure.Atlas;

/// <summary>
/// Atlas'a EYP teslim yöntemi (web servis, mesaj tablosu, klasör) netleşene kadar kullanılan sahte istemci.
/// Hiçbir yere göndermez; her paketi kabul edilmiş sayar ve sahte bir referans döner.
/// </summary>
public sealed class FakeAtlasEypTransferClient : IAtlasEypTransferClient
{
    public Task<AtlasEypTransferResultDto> SendAsync(AtlasEypSubmissionDto submission, CancellationToken cancellationToken)
    {
        return Task.FromResult(AtlasEypTransferResultDto.Success($"FAKE-{Guid.NewGuid():N}"));
    }
}
