namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Birimdeki bir personel ve oturum (login) durumu.
/// </summary>
public sealed class DepartmentUserLoginStatusDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string DepartmentShortName { get; set; }
    public int UserType { get; set; }
    public bool IsActive { get; set; }

    /// <summary>Son başarılı giriş JWT geçerlilik süresi içindeyse true.</summary>
    public bool IsLoggedIn { get; set; }

    /// <summary>Son başarılı giriş zamanı (UTC). Hiç giriş yapmamışsa null.</summary>
    public DateTime? LastLoginDate { get; set; }

    public string? LastLoginIpAddress { get; set; }
}
