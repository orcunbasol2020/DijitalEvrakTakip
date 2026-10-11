namespace DijitalEvrakTakip.Application.Options;

/// <summary>
/// Hangi giriş yollarının açık olduğu (appsettings 'Authentication' bölümü). Ortam adına (Development /
/// Production) bağlanmaz; canlı pod da 'development' ile çalışabiliyor. Varsayılan: yalnızca Windows girişi.
/// </summary>
public sealed class LoginSettings
{
    public const string SectionName = "Authentication";

    /// <summary>Windows oturumundaki kullanıcıyla (Kerberos) giriş için ASP.NET kimlik şeması.</summary>
    public const string WindowsScheme = "Negotiate";

    /// <summary>
    /// Kullanıcı adı + şifre ile giriş (api/Users/Login). Geliştirme ortamında başka kullanıcılarla test
    /// etmek içindir; local'de launchSettings.json'dan açılır, canlıda kapalı kalmalı.
    /// </summary>
    public bool FormLoginEnabled { get; set; }

    /// <summary>Windows oturumundaki kullanıcıyla otomatik giriş (api/Users/WindowsLogin).</summary>
    public bool WindowsLoginEnabled { get; set; } = true;
}
