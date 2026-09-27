using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.CreateOutgoingDocumentDistribution;

public sealed record CreateOutgoingDocumentDistributionCommand(
    Guid OutgoingDocumentId,
    IList<OutgoingDocumentDistributionRecipient> Recipients
) : IRequest<IList<OutgoingDocumentDistributionDto>>;

public sealed record OutgoingDocumentDistributionRecipient(
    Guid? DepartmentId,
    Guid? ExternalInstitutionId,
    bool? ActionRequired,
    DateTime? SentDate,
    DateTime? DeliveryDate,
    int? DeliveryMethod,
    string? Notes
);
