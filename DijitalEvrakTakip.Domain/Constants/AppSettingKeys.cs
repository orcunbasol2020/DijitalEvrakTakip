namespace DijitalEvrakTakip.Domain.Constants;

/// <summary>
/// AppSettings tablosundaki anahtarlar. Yeni ayar eklerken buraya sabit ekleyip
/// seed migration'ına da satır eklenmelidir; böylece yazım hatası riski kalmaz.
/// </summary>
public static class AppSettingKeys
{
    public const string ApplicationName = "ApplicationName";
    public const string SupportEmail = "SupportEmail";
    public const string SupportPhone = "SupportPhone";
    public const string AnnouncementMessage = "AnnouncementMessage";

    // Taranan PDF'lerin otomatik içe aktarımı
    public const string ScanImportFolderPath = "ScanImportFolderPath";
    public const string ScanImportEnabled = "ScanImportEnabled";
    public const string ScanImportIntervalSeconds = "ScanImportIntervalSeconds";

    public static readonly IReadOnlyList<string> All = new[]
    {
        ApplicationName,
        SupportEmail,
        SupportPhone,
        AnnouncementMessage,
        ScanImportFolderPath,
        ScanImportEnabled,
        ScanImportIntervalSeconds
    };
}
