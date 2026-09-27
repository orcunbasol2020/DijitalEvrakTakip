using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.UpdateOutgoingDocumentDistribution;

public sealed record UpdateOutgoingDocumentDistributionCommand(
    Guid Id,
    bool? ActionRequired,
    DateTime? SentDate,
    DateTime? DeliveryDate,
    int? DeliveryMethod,
    string? Notes
) : IRequest<OutgoingDocumentDistributionDto>;
