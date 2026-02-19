using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Commands.CreateUserRole;

public sealed record CreateUserRoleCommand
(
    string UserId,
        string RoleId
) : IRequest<MessageResponse>;

