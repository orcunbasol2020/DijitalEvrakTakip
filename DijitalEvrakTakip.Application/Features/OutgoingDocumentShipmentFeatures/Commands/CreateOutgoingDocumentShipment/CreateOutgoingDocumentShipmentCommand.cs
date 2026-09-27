using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Commands.CreateOutgoingDocumentShipment;

/// <summary>
/// Zimmetli kullanıcı bir veya birden fazla dağıtım satırını (aynı pakete giren evraklar)
/// kargoya verir. Kargo kaydı oluşur, dağıtım satırları pakete bağlanır, ilgili evrakların
/// aktif zimmeti "Kargoya Verildi" ile kapanır ve her evrağa "Gönderildi" işlemi yazılır.
/// EnvelopeId verilirse zarfın durumu da aynı işlemde "Kargoya Verildi" yapılır.
/// </summary>
public sealed record CreateOutgoingDocumentShipmentCommand(
    IList<Guid> DistributionIds,
    int CargoCompany,
    string TrackingNumber,
    Guid SentUserId,
    DateTime? SentDate,
    Guid? ExternalInstitutionId,
    string? RecipientName,
    decimal? Cost,
    string? Notes,
    Guid? EnvelopeId = null
) : IRequest<OutgoingDocumentShipmentDto>;
