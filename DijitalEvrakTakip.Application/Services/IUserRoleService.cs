using DijitalEvrakTakip.Application.Features.UserRoleFeatures.Commands.CreateUserRole;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using System.Linq.Expressions;

namespace DijitalEvrakTakip.Application.Services;

public interface IUserRoleService
{
    Task CreateAsync(CreateUserRoleCommand request, CancellationToken cancellationToken);
    Task<IList<UserRole>> GetUserRoleByUser(Expression<Func<UserRole, bool>> predicate, CancellationToken cancellationToken);
    Task<IList<string>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task AssignRolesAsync(Guid userId, IList<Guid> roleIds, CancellationToken cancellationToken);
    Task RemoveRoleFromUser(Guid userId, Guid roleId, CancellationToken cancellationToken);

}

