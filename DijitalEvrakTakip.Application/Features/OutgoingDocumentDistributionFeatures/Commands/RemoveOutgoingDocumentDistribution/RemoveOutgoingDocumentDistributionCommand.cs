using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.RemoveOutgoingDocumentDistribution;

public sealed record RemoveOutgoingDocumentDistributionCommand(
    Guid Id
) : IRequest<MessageResponse>;
