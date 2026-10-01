using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Commands.RemoveUserRole;

public sealed class RemoveUserRoleCommandHandler : IRequestHandler<RemoveUserRoleCommand, MessageResponse>
{
    private readonly IUserRoleService _userRoleService;

    public RemoveUserRoleCommandHandler(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public async Task<MessageResponse> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        await _userRoleService.RemoveRoleFromUser(request.UserId, request.RoleId, cancellationToken);

        return new("Kullanıcıdan Rol Başarıyla Kaldırıldı.");
    }
}
