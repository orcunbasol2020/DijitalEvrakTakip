using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetDepartmentUsersLoginStatus;

/// <summary>
/// Birimdeki personelleri, son başarılı girişlerine göre oturum (login) durumlarıyla birlikte getirir.
/// IncludeSubDepartments=true ise alt birimlerdeki personeller de dahil edilir.
/// OnlyLoggedIn=true ise yalnızca oturumu açık olanlar döner.
/// </summary>
public sealed record GetDepartmentUsersLoginStatusQuery(
    Guid DepartmentId,
    bool IncludeSubDepartments = true,
    bool OnlyActive = true,
    bool OnlyLoggedIn = false) : IRequest<IList<DepartmentUserLoginStatusDto>>;
