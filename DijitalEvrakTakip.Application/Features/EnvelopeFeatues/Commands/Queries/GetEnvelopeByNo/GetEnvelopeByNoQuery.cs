using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetEnvelopeByNo;

public sealed record GetEnvelopeByNoQuery(
    string EnvelopeNo
) : IRequest<EnvelopeDto?>;