using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Queries.GetAllRole;

public sealed class GetAllRoleHandler : IRequestHandler<GetAllRoleQuery, IList<Role>>
{
    private readonly IRoleService _roleService;

    public GetAllRoleHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IList<Role>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
    {
        IList<Role> roles = await _roleService.GetAllAsync(request, cancellationToken);
        return roles;
    }


}
