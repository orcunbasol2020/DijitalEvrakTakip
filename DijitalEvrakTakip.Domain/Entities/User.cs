namespace DijitalEvrakTakip.Domain.Entities;

/// <summary>
/// Kullanıcı; yalnızca okunur. vw_Users view'ından gelir: ComAuthorizationUser + kullanıcının ComAgency
/// kaydındaki birim (ComDepartment). Ad, e-posta, birim ve aktiflik kaynak sistemde yönetilir; bizde
/// yalnızca rol atamaları (UserRole) tutulur.
/// </summary>
public sealed class User
{
    /// <summary>ComAuthorizationUser.UserId</summary>
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }

    /// <summary>AD kullanıcı adı (sAMAccountName); NTLogonName'in "HITIT\" öneki atılmış hali.</summary>
    public string UserName { get; set; }

    /// <summary>ComDepartment.DepartmentId; kullanıcının ComAgency kaydında birim yoksa boş.</summary>
    public int? DepartmentId { get; set; }
    public Department Department { get; set; }

    /// <summary>Kaynakta aktif, girişi engellenmemiş ve kurumdan ayrılmamış (LeaveDate boş).</summary>
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
