namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// "Hakkında" ekranı için birleşik uygulama bilgisi.
/// Version ve BuildDate assembly'den, diğer alanlar AppSettings tablosundan gelir.
/// </summary>
public sealed record AppInfoDto(
    string Version,
    DateTime? BuildDate,
    string? ApplicationName,
    string? SupportEmail,
    string? SupportPhone,
    string? AnnouncementMessage
);
