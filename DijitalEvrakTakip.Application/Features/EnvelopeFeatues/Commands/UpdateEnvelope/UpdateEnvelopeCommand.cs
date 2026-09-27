using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.UpdateEnvelope;

/// <summary>
/// Zarf etiketindeki alıcı kurum, alıcı (UnitName) ve adres bilgisini günceller.
/// Null gelen alan değiştirilmez; boş metin gelirse alan temizlenir.
/// ExternalInstitutionId verilirse zarfın hedefi o dış kurum / misyon olur ve
/// kurum içi hedef (TargetDepartmentId) temizlenir; ikisi birlikte dolu olmaz.
/// </summary>
public sealed record UpdateEnvelopeCommand(
    Guid Id,
    string? UnitName,
    string? Address,
    Guid? ExternalInstitutionId = null
) : IRequest<MessageResponse>;
