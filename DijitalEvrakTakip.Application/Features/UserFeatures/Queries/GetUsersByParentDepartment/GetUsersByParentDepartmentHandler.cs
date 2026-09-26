using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUsersByParentDepartment;

public sealed class GetUsersByParentDepartmentHandler
    : IRequestHandler<GetUsersByParentDepartmentQuery, IList<UserDto>>
{
    private readonly IUserService _userService;
    private readonly IDepartmentService _departmentService;

    public GetUsersByParentDepartmentHandler(
        IUserService userService,
        IDepartmentService departmentService)
    {
        _userService = userService;
        _departmentService = departmentService;
    }

    public async Task<IList<UserDto>> Handle(
        GetUsersByParentDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        IList<Guid> departmentIds = await _departmentService.GetDescendantIdsAsync(
            request.DepartmentId,
            request.IncludeSelf,
            cancellationToken);

        if (departmentIds.Count == 0)
            return new List<UserDto>();

        IQueryable<User> query = _userService
            .GetAll()
            .Include(u => u.Department)
            .Where(u => !u.IsDeleted && departmentIds.Contains(u.DepartmentId));

        if (request.OnlyActive)
            query = query.Where(u => u.IsActive);

        return await query
            .ProjectToType<UserDto>()
            .OrderBy(u => u.DepartmentName)
            .ThenBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }
}
