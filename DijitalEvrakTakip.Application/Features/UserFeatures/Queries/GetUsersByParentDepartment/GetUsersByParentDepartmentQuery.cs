using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUsersByParentDepartment;

/// <summary>
/// Yöneticinin bağlı olduğu departman (DepartmentId) ve bu departmanın altındaki tüm
/// alt departmanlardaki (Department.ParentId zinciri) personelleri getirir.
/// IncludeSelf=false verilirse yalnızca alt departmanlardaki personeller döner.
/// </summary>
public sealed record GetUsersByParentDepartmentQuery(
    Guid DepartmentId,
    bool IncludeSelf = true,
    bool OnlyActive = true) : IRequest<IList<UserDto>>;
