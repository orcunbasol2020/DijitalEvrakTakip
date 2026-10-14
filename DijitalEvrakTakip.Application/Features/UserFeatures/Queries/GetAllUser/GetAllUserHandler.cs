using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser
{
    public sealed class GetAllUserHandler : IRequestHandler<GetAllUserQuery, IList<UserDto>>
    {
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;

        public GetAllUserHandler(IUserService userService, IDepartmentService departmentService)
        {
            _userService = userService;
            _departmentService = departmentService;
        }

        public async Task<IList<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _userService.GetAll().Where(u => !u.IsDeleted);
            bool filtered = false;

            if (request.DepartmentId.HasValue)
            {
                if (request.IncludeSubDepartments)
                {
                    // Birim ağacı tek sorguda çekilir; alt birimler için ayrı sorgu atılmaz.
                    IList<int> departmentIds = await _departmentService.GetDescendantIdsAsync(
                        request.DepartmentId.Value, includeSelf: true, cancellationToken);
                    query = query.Where(u => u.DepartmentId.HasValue && departmentIds.Contains(u.DepartmentId.Value));
                }
                else
                {
                    query = query.Where(u => u.DepartmentId == request.DepartmentId.Value);
                }
                filtered = true;
            }

            if (request.RoleId.HasValue)
            {
                Guid roleId = request.RoleId.Value;
                query = query.Where(u => u.UserRoles.Any(ur =>
                    ur.RoleId == roleId && !ur.IsDeleted && !ur.Role.IsDeleted && ur.Role.IsActive));
                filtered = true;
            }

            if (request.OnlyWithRoles)
            {
                query = query.Where(u => u.UserRoles.Any(ur =>
                    !ur.IsDeleted && !ur.Role.IsDeleted && ur.Role.IsActive));
                filtered = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = _userService.Search(query, request.Search);
                filtered = true;
            }

            IQueryable<User> ordered = query.OrderBy(u => u.Name).ThenBy(u => u.Surname);
            if (!filtered)
                ordered = ordered.Take(GetAllUserQuery.MaxUnfilteredCount);

            // Roller aynı sorguda projekte edilir; kullanıcı başına ayrı sorgu atılmaz.
            var result = await ordered
                .Select(u => new UserDto
                {
                    Id = u.Id.ToString(),
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                    UserName = u.UserName,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department.Name,
                    DepartmentShortName = u.Department.ShortName,
                    TitleName = u.TitleName,
                    IsActive = u.IsActive,
                    IsDeleted = u.IsDeleted,
                    Roles = u.UserRoles
                        .Where(ur => !ur.IsDeleted && !ur.Role.IsDeleted && ur.Role.IsActive)
                        .OrderBy(ur => ur.Role.Name)
                        .Select(ur => new RoleSummaryDto { Id = ur.Role.Id, Name = ur.Role.Name })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
