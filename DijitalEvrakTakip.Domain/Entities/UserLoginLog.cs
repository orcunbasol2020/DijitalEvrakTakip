using DijitalEvrakTakip.Domain.Abstractions;
using DijitalEvrakTakip.Domain.Enums;

namespace DijitalEvrakTakip.Domain.Entities;

/// <summary>
/// Her login denemesini (başarılı veya başarısız) kaydeder.
/// </summary>
public sealed class UserLoginLog : Entity
{
    /// <summary>Kullanıcı bulunduysa Id'si, bulunamadıysa null.</summary>
    public Guid? UserId { get; set; }

    /// <summary>Login formunda girilen kullanıcı adı (kullanıcı bulunamasa da kaydedilir).</summary>
    public string UserName { get; set; }

    public bool IsSuccess { get; set; }

    public LoginFailureReasonEnum? FailureReason { get; set; }

    public string IpAddress { get; set; }

    public string UserAgent { get; set; }

    public DateTime LoginDate { get; set; }

    // Navigation
    public User User { get; set; }
}
