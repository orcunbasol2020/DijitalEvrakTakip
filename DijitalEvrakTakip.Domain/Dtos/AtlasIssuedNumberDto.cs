namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Atlas EBYS'nin verdiği tek bir evrak numarası.
/// </summary>
public sealed record AtlasIssuedNumberDto(
    string QrCode,
    string? AtlasReferenceId
);
