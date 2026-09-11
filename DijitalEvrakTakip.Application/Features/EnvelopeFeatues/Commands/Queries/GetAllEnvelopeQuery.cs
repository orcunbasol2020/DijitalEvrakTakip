using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetAllEnvelope;

public sealed record GetAllEnvelopeQuery()
    : IRequest<IList<EnvelopeDocumentCountDto>>;