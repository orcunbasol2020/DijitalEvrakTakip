using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Queries.GetEnvelopeDocumentsByEnvelopeId;

public sealed record GetEnvelopeDocumentsByEnvelopeIdQuery(
    Guid EnvelopeId
) : IRequest<IList<EnvelopeDocumentDto>>;