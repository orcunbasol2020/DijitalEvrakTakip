using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.UpdateRole;

/// <summary>
/// Rolün adını ve aktif / pasif durumunu günceller; tam gövde beklenir.
/// Değişiklik, rol sahibi kullanıcılarda bir sonraki girişte (yeni token) geçerli olur.
/// </summary>
public sealed record UpdateRoleCommand(
    Guid Id,
    string Name,
    bool IsActive
    ) : IRequest<MessageResponse>;
