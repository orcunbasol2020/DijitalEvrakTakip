using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    bool IsActive
    ) : IRequest<MessageResponse>;
