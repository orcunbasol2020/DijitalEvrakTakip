using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Queries.GetAllEnvelopeDocument;

public sealed record GetAllEnvelopeDocumentQuery()
    : IRequest<IList<EnvelopeDocumentDto>>;