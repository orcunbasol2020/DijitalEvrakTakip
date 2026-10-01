using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetAllEnvelope;

public sealed record GetAllEnvelopeQuery(Guid? CreatedByUserId = null, Guid? DepartmentId = null)
    : IRequest<IList<EnvelopeDocumentCountDto>>;