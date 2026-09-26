using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities;

/// <summary>
/// Çalışma zamanında arayüzden yönetilen uygulama ayarları (anahtar-değer).
/// Versiyon ve build tarihi gibi koda ait bilgiler burada tutulmaz; assembly'den okunur.
/// </summary>
public class AppSetting : Entity
{
    /// <summary>Benzersiz anahtar. Kodda <see cref="Constants.AppSettingKeys"/> sabitleriyle eşleşir.</summary>
    public string Key { get; set; }

    public string? Value { get; set; }

    /// <summary>Admin ekranında gösterilecek açıklama.</summary>
    public string? Description { get; set; }

    /// <summary>Değeri son değiştiren kullanıcı.</summary>
    public Guid? UpdatedByUserId { get; set; }
}
