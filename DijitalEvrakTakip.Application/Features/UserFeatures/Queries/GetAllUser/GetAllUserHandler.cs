using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser
{
    public sealed class GetAllUserHandler : IRequestHandler<GetAllUserQuery, IList<UserDto>>
    {
        private readonly IUserService _userService;

        public GetAllUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IList<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            //IList<UserDto> users = await _userService.GetAllAsync(request, cancellationToken);
            //return users.OrderBy(u => u.Name).ToList();

            IQueryable<User> query = _userService.GetAll();

            if (request.DepartmentId.HasValue)
            {
                query = query.Where(u => u.DepartmentId == request.DepartmentId.Value);
            }

            if (request.RoleId.HasValue)
            {
                Guid roleId = request.RoleId.Value;
                query = query.Where(u => u.UserRoles.Any(ur =>
                    ur.RoleId == roleId && !ur.IsDeleted && !ur.Role.IsDeleted && ur.Role.IsActive));
            }

            // Roller aynı sorguda projekte edilir; kullanıcı başına ayrı sorgu atılmaz.
            var result = await query
                .OrderBy(u => u.Name)
                .Select(u => new UserDto
                {
                    Id = u.Id.ToString(),
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                    UserName = u.UserName,
                    DepartmentId = u.DepartmentId.ToString(),
                    DepartmentName = u.Department.Name,
                    DepartmentShortName = u.Department.ShortName,
                    UserType = u.UserType,
                    IsActive = u.IsActive,
                    IsDeleted = u.IsDeleted,
                    CreatedDate = u.CreatedDate,
                    UpdateDate = u.UpdateDate,
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
