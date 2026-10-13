using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities;

/// <summary>
/// Kullanıcının DISNET'teki birimi. Kaynak sistemde (ComAuthorizationUser.AgencyId -> ComAgency.DepartmentId) çoğu kişi
/// yalnızca üst birimde (ör. BTGY) görünür; DISNET ise alt birimi (ör. BDHD Fiziki Evrak Birimi) bilir. Burada kayıt varsa
/// vw_Users.DepartmentId bunu, yoksa ComAgency'deki birimi verir. Com kopyaları tazelendiğinde bu tablo korunur.
/// </summary>
public sealed class UserDepartmentAssignment : Entity
{
    /// <summary>ComAuthorizationUser.UserId</summary>
    public Guid UserId { get; set; }

    /// <summary>ComDepartment.DepartmentId</summary>
    public int DepartmentId { get; set; }

    /// <summary>Atamanın nereden geldiği, ör. "DISNET 2026-10-12 btgm.txt".</summary>
    public string Source { get; set; }
}
