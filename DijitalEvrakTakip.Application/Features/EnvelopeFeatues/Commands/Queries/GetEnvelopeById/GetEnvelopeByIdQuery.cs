using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetEnvelopeById;

public sealed record GetEnvelopeByIdQuery(
    Guid Id
) : IRequest<EnvelopeDto?>;