using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.UpdateRole;

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, MessageResponse>
{
    private readonly IRoleService _roleService;

    public UpdateRoleCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<MessageResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        await _roleService.UpdateAsync(request, cancellationToken);
        return new("Rol başarıyla güncellendi.");
    }
}
