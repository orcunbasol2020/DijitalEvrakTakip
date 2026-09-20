using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Commands.AssignRolesToUser;

public sealed record AssignRolesToUserCommand(
    Guid UserId,
    IList<Guid> RoleIds
) : IRequest<MessageResponse>;
