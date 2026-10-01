using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Commands.AssignRolesToUser;

public sealed class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, MessageResponse>
{
    private readonly IUserRoleService _userRoleService;

    public AssignRolesToUserCommandHandler(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public async Task<MessageResponse> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
    {
        await _userRoleService.AssignRolesAsync(request.UserId, request.RoleIds, cancellationToken);

        return new("Kullanıcıya Roller Başarıyla Atandı.");
    }
}
