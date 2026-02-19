using DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.CreateRole;
using DijitalEvrakTakip.Application.Features.RoleFeatures.Queries.GetAllRole;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IRoleService
{
    Task CreateAsync(CreateRoleCommand request, CancellationToken cancellationToken);
    //public IQueryable<Role> GetAll();
    Task<IList<Role>> GetAllAsync(GetAllRoleQuery request, CancellationToken cancellationToken);
}
