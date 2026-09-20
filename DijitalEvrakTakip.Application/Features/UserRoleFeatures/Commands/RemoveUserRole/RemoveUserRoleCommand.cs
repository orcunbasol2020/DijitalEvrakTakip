using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Commands.RemoveUserRole;

public sealed record RemoveUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest<MessageResponse>;
