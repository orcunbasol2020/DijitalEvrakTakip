using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser;

/// <param name="Search">Ad, soyad ya da kullanıcı adında geçen metin.</param>
/// <param name="OnlyWithRoles">true: yalnızca bu sistemde en az bir rolü olan kullanıcılar.</param>
/// <param name="IncludeSubDepartments">
/// true: DepartmentId ile birlikte ona ParentId zinciriyle bağlı tüm alt birimlerdeki kullanıcılar da gelir.
/// DepartmentId verilmezse etkisi yoktur.
/// </param>
/// <remarks>
/// Kaynakta binlerce kullanıcı var; hiçbir süzgeç verilmezse en fazla MaxUnfilteredCount kullanıcı döner.
/// </remarks>
public sealed record GetAllUserQuery(
    int? DepartmentId = null,
    Guid? RoleId = null,
    string? Search = null,
    bool OnlyWithRoles = false,
    bool IncludeSubDepartments = false) : IRequest<IList<UserDto>>
{
    public const int MaxUnfilteredCount = 200;
}
