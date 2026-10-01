namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Bir kullanıcının son başarılı girişine göre hesaplanan oturum durumu.
/// </summary>
public sealed class UserLoginStatusDto
{
    public Guid UserId { get; set; }

    /// <summary>Son başarılı giriş zamanı (UTC). Hiç giriş yapmamışsa null.</summary>
    public DateTime? LastLoginDate { get; set; }

    public string? LastLoginIpAddress { get; set; }

    /// <summary>
    /// Son başarılı giriş JWT geçerlilik süresi (Jwt:ExpirationMinutes) içindeyse true.
    /// Çıkış (logout) kaydı tutulmadığı için token geçerlilik penceresi esas alınır.
    /// </summary>
    public bool IsLoggedIn { get; set; }
}
