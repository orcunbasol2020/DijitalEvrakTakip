using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetDepartmentUsersLoginStatus;

public sealed class GetDepartmentUsersLoginStatusHandler
    : IRequestHandler<GetDepartmentUsersLoginStatusQuery, IList<DepartmentUserLoginStatusDto>>
{
    private readonly IUserService _userService;
    private readonly IDepartmentService _departmentService;
    private readonly IUserLoginLogService _loginLogService;

    public GetDepartmentUsersLoginStatusHandler(
        IUserService userService,
        IDepartmentService departmentService,
        IUserLoginLogService loginLogService)
    {
        _userService = userService;
        _departmentService = departmentService;
        _loginLogService = loginLogService;
    }

    public async Task<IList<DepartmentUserLoginStatusDto>> Handle(
        GetDepartmentUsersLoginStatusQuery request,
        CancellationToken cancellationToken)
    {
        IList<Guid> departmentIds = request.IncludeSubDepartments
            ? await _departmentService.GetDescendantIdsAsync(request.DepartmentId, includeSelf: true, cancellationToken)
            : new List<Guid> { request.DepartmentId };

        if (departmentIds.Count == 0)
            return new List<DepartmentUserLoginStatusDto>();

        IQueryable<User> query = _userService
            .GetAll()
            .AsNoTracking()
            .Include(u => u.Department)
            .Where(u => !u.IsDeleted && departmentIds.Contains(u.DepartmentId));

        if (request.OnlyActive)
            query = query.Where(u => u.IsActive);

        List<DepartmentUserLoginStatusDto> users = await query
            .Select(u => new DepartmentUserLoginStatusDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                UserName = u.UserName,
                Email = u.Email,
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department != null ? u.Department.Name : null,
                DepartmentShortName = u.Department != null ? u.Department.ShortName : null,
                UserType = u.UserType,
                IsActive = u.IsActive
            })
            .ToListAsync(cancellationToken);

        if (users.Count == 0)
            return users;

        IDictionary<Guid, UserLoginStatusDto> statuses = await _loginLogService.GetLoginStatusesAsync(
            users.Select(u => u.Id).ToList(),
            cancellationToken);

        foreach (DepartmentUserLoginStatusDto user in users)
        {
            if (!statuses.TryGetValue(user.Id, out UserLoginStatusDto? status))
                continue;

            user.IsLoggedIn = status.IsLoggedIn;
            user.LastLoginDate = status.LastLoginDate;
            user.LastLoginIpAddress = status.LastLoginIpAddress;
        }

        IEnumerable<DepartmentUserLoginStatusDto> result = users;
        if (request.OnlyLoggedIn)
            result = result.Where(u => u.IsLoggedIn);

        // Oturumu açık olanlar önce, ardından birim ve ada göre.
        return result
            .OrderByDescending(u => u.IsLoggedIn)
            .ThenBy(u => u.DepartmentName)
            .ThenBy(u => u.Name)
            .ThenBy(u => u.Surname)
            .ToList();
    }
}
