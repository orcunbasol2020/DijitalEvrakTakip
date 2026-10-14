using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.DeleteRole;

/// <summary>
/// Rolü silindi olarak işaretler (fiziksel silme yok). Rol bir kullanıcıya atanmışsa silinmez.
/// </summary>
public sealed record DeleteRoleCommand(Guid Id) : IRequest<MessageResponse>;
