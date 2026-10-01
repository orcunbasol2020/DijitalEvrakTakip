using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.UpdateEnvelopeStatus;

public sealed record UpdateEnvelopeStatusCommand(
    Guid Id,
    int Status
) : IRequest<MessageResponse>;
