using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Tamamlanmış EYP paketini Atlas'a teslim eder. Teslim yöntemi (web servis, mesaj tablosu, klasör)
/// netleşene kadar FakeAtlasEypTransferClient kullanılır.
/// </summary>
public interface IAtlasEypTransferClient
{
    /// <summary>true ise paketler Atlas'a gönderilmez, başarılı sayılır (sahte istemci).</summary>
    bool IsSimulated { get; }

    /// <summary>
    /// Paketi gönderir. Atlas'a erişilemiyorsa istisna fırlatabilir veya
    /// <see cref="AtlasEypTransferResultDto.TransientFailure"/> dönebilir; ikisi de tekrar denenir.
    /// Atlas paketi reddederse <see cref="AtlasEypTransferResultDto.Rejected"/> döner.
    /// </summary>
    Task<AtlasEypTransferResultDto> SendAsync(AtlasEypSubmissionDto submission, CancellationToken cancellationToken);
}
