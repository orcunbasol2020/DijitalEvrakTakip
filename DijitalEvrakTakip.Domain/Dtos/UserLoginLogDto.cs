namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class UserLoginLogDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    /// <summary>Login formunda girilen kullanıcı adı.</summary>
    public string UserName { get; set; }

    /// <summary>Kullanıcı bulunduysa ad soyad, aksi halde null.</summary>
    public string FullName { get; set; }

    public string DepartmentName { get; set; }
    public bool IsSuccess { get; set; }

    /// <summary>Enum adı (UserNotFound, InvalidPassword, ...). Başarılı girişte null.</summary>
    public string FailureReason { get; set; }

    /// <summary>Türkçe açıklama (Kullanıcı Bulunamadı, Şifre Hatalı, ...). Başarılı girişte null.</summary>
    public string FailureReasonText { get; set; }

    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime LoginDate { get; set; }
}
